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
            var rat = RatUtils.CreateRat(name);
            
            using (StreamReader streamReader =  new  StreamReader(myBlob))
            {
                requestBody = await streamReader.ReadToEndAsync();
                streamReader.BaseStream.Position = 0;

                var imageDetails = ImageUtils.GetExifData(streamReader.BaseStream, log);
                

                if(imageDetails.captureDate != DateTime.MinValue)
                    rat.Metadata.SourceDateTime = imageDetails.captureDate;

                var result = CustomVisionService.GetPredictionResult(streamReader.BaseStream, log);
                // Loop over each prediction and write out the results
                
                Console.WriteLine(imageDetails.captureDate);
                foreach (var c in result.Predictions)
                {
                    Console.WriteLine($"\t{c.TagName}: {c.Probability:P1}");
                }
            }
        }
    }
}
