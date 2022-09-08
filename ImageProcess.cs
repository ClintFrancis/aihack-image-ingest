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
                // requestBody = await streamReader.ReadToEndAsync();
                metaData = GetExifData(streamReader.BaseStream, log);
            }

            return new OkObjectResult(metaData);
        }

        private static MetaData GetExifData(Stream fileStream, ILogger log)
        {
            var result = new MetaData();
            try
            {
                using (var exifReader = new ExifReader(fileStream))
                {
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
                }
            }
            catch (Exception e)
            {
                log.LogInformation(e.Message);
            }

            return result;
        }
    }
}
