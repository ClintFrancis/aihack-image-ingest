using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Documents.Client;
using System.Linq;
using Microsoft.Azure.WebJobs.Host;
using System.Collections.Generic;

namespace Datacom.Envirohack
{
    public static class ImageProcess
    {
        [FunctionName("process_image")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = String.Empty;
            
            using (StreamReader streamReader =  new  StreamReader(req.Body))
            {
                requestBody = await streamReader.ReadToEndAsync();
                streamReader.BaseStream.Position = 0;
                var imageDetails = ImageUtils.GetExifData(streamReader.BaseStream, log);
                if(imageDetails.captureDate == DateTime.MinValue)
                {
                    // The result was bad
                    log.LogInformation($"Image has no capture date");
                }

                var result = CustomVisionService.GetPredictionResult(streamReader.BaseStream, log);
                // Loop over each prediction and write out the results
                foreach (var c in result.Predictions)
                {
                    Console.WriteLine($"\t{c.TagName}: {c.Probability:P1}");
                }
            }

            // TODO connect to CosmosDB

            return new OkObjectResult("Image Processed");
        }

        [FunctionName("location")]
        public static async Task<IActionResult> Location(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // get name query from request
            string name = req.Query["name"];
            var location = RatUtils.GetCameraLocation(name);

            return new OkObjectResult(location);
        }

        [FunctionName("merged_data")]
        public static async Task<IActionResult> MergedData(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log,
            [CosmosDB(
                databaseName: Constants.DbMain,
                collectionName: Constants.DbCollectionFauna,
                ConnectionStringSetting = "CosmosDBConnection")] DocumentClient client)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            
            // request the first 10 documents from the faun container in cosmosdb
            var result = client.CreateDocumentQuery<Fauna>(UriFactory.CreateDocumentCollectionUri(Constants.DbMain, Constants.DbCollectionFauna), new FeedOptions { MaxItemCount = 10 });
            
            var response = new List<MergedData>();
            foreach (var item in result)
            {
                var captureDate = item.Media.CaptureDateTime;

                // get date range of 6 hours before and after the capture date
                var intervalSize = 6;
                var startDate = captureDate.AddHours(-intervalSize);
                var endDate = captureDate.AddHours(intervalSize);
                
                // find matching weather data for the capture date within the hour from the weather container in cosmosdb
                var weatherResult = client.CreateDocumentQuery<Weather>(UriFactory.CreateDocumentCollectionUri(Constants.DbMain, Constants.DbCollectionWeather), new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = 10 })
                    .Where(w => w.DateTime >= startDate && w.DateTime <= endDate).AsEnumerable().FirstOrDefault();

                // if no weather data is found, skip this item
                if (weatherResult == null)
                {
                    Console.WriteLine("no results");
                } else  {
                    Console.WriteLine(weatherResult);
                }
                var merged = new MergedData(item, weatherResult);
                response.Add(merged);
            }

            return new OkObjectResult(response);
        }
    }
}
