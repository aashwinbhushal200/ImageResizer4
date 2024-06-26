using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ImageHandler
{
    public static class PhotosDownload
    {
        [FunctionName("PhotosDownload")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get",Route = "photos/{id}")] HttpRequest req,
            [Blob("photos-small/{id}.jpg", FileAccess.Read, Connection =Literals.StorageConnectionString)] Stream imageSmall, 
            [Blob("photos-medium/{id}.jpg", FileAccess.Read, Connection =Literals.StorageConnectionString)] Stream imageMedium, 
            [Blob("photos/{id}.jpg",FileAccess.Read, Connection = Literals.StorageConnectionString)] Stream imageOriginal,
            Guid id, ILogger log)
        {
            log.LogInformation("download starting");

            byte[] data;

            if (req.Query["size"] == "sm")
            {
                log?.LogInformation("Retrieving the small size");
                data = await GetBytesFromStreamAsync(imageSmall);
            }
            else if (req.Query["size"] == "md")
            {
                log?.LogInformation("Retrieving the medium size");
                data = await GetBytesFromStreamAsync(imageMedium);
            }
            else
            {
                log?.LogInformation("Retrieving the original size");
                data = await GetBytesFromStreamAsync(imageOriginal);
            }

            return new FileContentResult(data, "image/jpeg")
            {
                FileDownloadName = $"{id}.jpg"
            };
        }
        private static async Task<byte[]> GetBytesFromStreamAsync(Stream stream)
        {
            byte[] data = new byte[stream.Length];
            await stream.ReadAsync(data, 0, data.Length);
            return data;
        }
    }
}
