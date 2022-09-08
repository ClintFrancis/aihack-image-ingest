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
            // StreamReader streamReader = new StreamReader(req.Body);
            using (StreamReader streamReader =  new  StreamReader(req.Body))
            {
                requestBody = await streamReader.ReadToEndAsync();
                streamReader.BaseStream.Position = 0;
                metaData = GetExifData(streamReader.BaseStream, log);

                var result = GetPredictionResult(streamReader.BaseStream, log);
                // Loop over each prediction and write out the results
                foreach (var c in result.Predictions)
                {
                    Console.WriteLine($"\t{c.TagName}: {c.Probability:P1}");
                }
            }

            return new OkObjectResult(metaData);
        }

        private static CustomVisionPredictionClient AuthenticatePrediction(string endpoint, string predictionKey)
        {
            // Create a prediction endpoint, passing in the obtained prediction key
            CustomVisionPredictionClient predictionApi = new CustomVisionPredictionClient(new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.ApiKeyServiceClientCredentials(predictionKey))
            {
                Endpoint = endpoint
            };
            return predictionApi;
        }

        private static ImagePrediction GetPredictionResult(Stream fileStream, ILogger log)
        {
            fileStream.Position = 0;

            // Submit image stream to Azure Custom Vision Service
            var predictionEndpoint = Environment.GetEnvironmentVariable("CognitiveServicesEndpoint");
            var predictionKey = Environment.GetEnvironmentVariable("CognitiveServicesKey");
            var projectId = new Guid(Environment.GetEnvironmentVariable("CognitiveServicesProjectId"));
            var publishedModelName = Environment.GetEnvironmentVariable("CognitiveServicesPublishedName");
                
            CustomVisionPredictionClient predictionApi = AuthenticatePrediction(predictionEndpoint, predictionKey);
            var result = predictionApi.ClassifyImage(projectId, publishedModelName, fileStream);

            return result;
        }

        private static MetaData GetExifData(Stream fileStream, ILogger log)
        {
            fileStream.Position = 0;
            var result = new MetaData();
            try
            {
                var exifReader = new ExifReader(fileStream);
                // using (var exifReader = new ExifReader(fileStream))
                // {
                    // The Double[] value contains the degrees, minutes and seconds.
                    string dateTimeOriginal;
                    
                    exifReader.GetTagValue(ExifTags.DateTimeOriginal, out dateTimeOriginal);

                    if (string.IsNullOrEmpty(dateTimeOriginal))
                    {
                        log.LogInformation("No date time available");
                    }
                    else
                    {
                        result.DateTimeOriginal = DateTime.ParseExact(dateTimeOriginal, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture);
                        result.IsDaytime = result.DateTimeOriginal.Hour > 6 && result.DateTimeOriginal.Hour < 18;
                    }
                // }
            }
            catch (Exception e)
            {
                log.LogInformation(e.Message);
            }

            return result;
        }
    }
}
