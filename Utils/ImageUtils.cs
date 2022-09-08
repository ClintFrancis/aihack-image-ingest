using System;
using System.IO;
using System.Globalization;
using Microsoft.Extensions.Logging;
using ExifLib;

namespace Datacom.Envirohack
{
    public static class ImageUtils
    {
        public static (DateTime captureDate, bool isDayTime) GetExifData(Stream fileStream, ILogger log)
        {
            fileStream.Position = 0;
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
                    var date = DateTime.ParseExact(dateTimeOriginal, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture);
                    return (
                        date,
                        date.Hour > 6 && date.Hour < 18
                    );
                }
            }
            catch (Exception e)
            {
                log.LogInformation(e.Message);
            }

            return (DateTime.MinValue, true);
        }
    }
}