using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Globalization;
using Newtonsoft.Json;
using ExifLib;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.WebJobs.Extensions.Files;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;

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
            MetaData metaData;
            
            using (StreamReader streamReader =  new  StreamReader(req.Body))
            {
                requestBody = await streamReader.ReadToEndAsync();
                streamReader.BaseStream.Position = 0;
                metaData = ImageUtils.GetExifData(streamReader.BaseStream, log);

                var result = CustomVisionService.GetPredictionResult(streamReader.BaseStream, log);
                // Loop over each prediction and write out the results
                foreach (var c in result.Predictions)
                {
                    Console.WriteLine($"\t{c.TagName}: {c.Probability:P1}");
                }
            }

            // TODO connect to CosmosDB

            return new OkObjectResult(metaData);
        }

        [FunctionName("location")]
        public static async Task<IActionResult> Location(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // get name query from request
            string name = req.Query["name"];
            var location = RatCameraLocationUtils.GetCameraLocation(name);

            return new OkObjectResult(location);
        }
    }
}
