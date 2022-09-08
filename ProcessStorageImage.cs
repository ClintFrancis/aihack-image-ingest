using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Datacom.Envirohack
{
    public class ProcessStorageImage
    {
        [FunctionName("ProcessStorageImage")]
        public async Task Run([BlobTrigger("images/{name}", Connection = "heinrichvandenheeverstrg_STORAGE")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            string requestBody = String.Empty;
            MetaData metaData;

            // Get CameraLocation from Blob Name
            var cameraLocaiton = RatCameraLocationUtils.GetCameraLocation(name);
            // write name and location to console
            Console.WriteLine($"Name: {name}, Camera Location: {cameraLocaiton}");
            
            using (StreamReader streamReader =  new  StreamReader(myBlob))
            {
                requestBody = await streamReader.ReadToEndAsync();
                streamReader.BaseStream.Position = 0;
                metaData = ImageUtils.GetExifData(streamReader.BaseStream, log);

                var result = CustomVisionService.GetPredictionResult(streamReader.BaseStream, log);
                // Loop over each prediction and write out the results
                
                Console.WriteLine(metaData.DateTimeOriginal);
                foreach (var c in result.Predictions)
                {
                    Console.WriteLine($"\t{c.TagName}: {c.Probability:P1}");
                }
            }
        }
    }
}
