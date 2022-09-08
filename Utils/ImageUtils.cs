using System;
using System.IO;
using System.Globalization;
using Microsoft.Extensions.Logging;
using ExifLib;

namespace Datacom.Envirohack
{
    public static class ImageUtils
    {
        public static MetaData GetExifData(Stream fileStream, ILogger log)
        {
            fileStream.Position = 0;
            var result = new MetaData();
            try
            {
                var exifReader = new ExifReader(fileStream);
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
            catch (Exception e)
            {
                log.LogInformation(e.Message);
            }

            return result;
        }
    }
}