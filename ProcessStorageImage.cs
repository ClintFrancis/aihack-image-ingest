using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Documents.Client;

namespace Datacom.Envirohack
{
    public class ProcessStorageImage
    {
        [FunctionName("ProcessStorageImage")]
        public async Task Run(
            [BlobTrigger("images/{name}", Connection = "heinrichvandenheeverstrg_STORAGE")]Stream myBlob,
             string name, 
             ILogger log,
             [CosmosDB(
                databaseName: Constants.DbMain,
                collectionName: Constants.DbCollectionFauna,
                ConnectionStringSetting = "CosmosDBConnection")] DocumentClient client)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            string requestBody = String.Empty;
            var rat = RatUtils.CreateRat(name);
            rat.Media = new Media{
                MediaType = "Image",
                Source = "Camera",
                Filename = name
            };
            
            using (StreamReader streamReader =  new  StreamReader(myBlob))
            {
                requestBody = await streamReader.ReadToEndAsync();
                streamReader.BaseStream.Position = 0;

                var imageDetails = ImageUtils.GetExifData(streamReader.BaseStream, log);
                if(imageDetails.captureDate != DateTime.MinValue){
                    rat.Metadata.SourceDateTime = rat.Media.CaptureDateTime = imageDetails.captureDate;
                    rat.Metadata.IsDaylight = rat.Media.IsDaylight = imageDetails.isDayTime;
                }
                    
                var result = CustomVisionService.GetPredictionResult(streamReader.BaseStream, log);

                // find TagName = "Rat" and set the confidence
                foreach (var prediction in result.Predictions)
                {
                    if(prediction.TagName == "rat"){
                        rat.Confidence = prediction.Probability;
                    }
                }
            }

            // Write the rat to the database
            if(rat.Confidence > 0.8){
                client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(Constants.DbMain, Constants.DbCollectionFauna), rat).Wait();
            }
        }
    }
}
