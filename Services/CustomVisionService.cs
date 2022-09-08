using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;

namespace Datacom.Envirohack {
    public static class CustomVisionService {

        public static ImagePrediction GetPredictionResult(Stream fileStream, ILogger log)
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

        private static CustomVisionPredictionClient AuthenticatePrediction(string endpoint, string predictionKey)
        {
            // Create a prediction endpoint, passing in the obtained prediction key
            CustomVisionPredictionClient predictionApi = new CustomVisionPredictionClient(new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.ApiKeyServiceClientCredentials(predictionKey))
            {
                Endpoint = endpoint
            };
            return predictionApi;
        }
    }
}