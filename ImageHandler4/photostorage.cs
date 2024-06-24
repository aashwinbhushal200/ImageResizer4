using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ImageHandler.Models;
using Azure.Storage.Blobs;
using AnalyzerService.Abstraction;

namespace ImageHandler
{
    public  class photostorage
    {
        private readonly IAnalyzerService analyzerService;

        public photostorage(IAnalyzerService analyzerService)
        {
            this.analyzerService = analyzerService;
        }
        [FunctionName("photostorage")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous,"post")] HttpRequest req,
        [Blob("photos", FileAccess.ReadWrite, Connection = Literals.StorageConnectionString)] BlobContainerClient blobContainer,
        //cosmos db ouput binding:
        [CosmosDB("photos",
                      "metadata",
                      Connection = Literals.CosmosDBConnection,
                      CreateIfNotExists = true)] IAsyncCollector<dynamic> items,
         ILogger _logger)

        {
            _logger?.LogInformation("C# HTTP trigger function processed a request.");
            //read the body and get request
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<PhotoUploadModel>(body);
            //new blobName to store:
            var newId = Guid.NewGuid();
            var blobName = $"{newId}.jpg";
            //make sure blob exists:
            await blobContainer.CreateIfNotExistsAsync();
            //get blobReference for cloudBlockBlob
            var cloudBlockBlob = blobContainer.GetBlobClient(blobName);
            //get the bytes of photofrom request object:
            var photoBytes = Convert.FromBase64String(request.Photo);
            //upload byte photo to cloudBLockBlob;
            using var photoStream = new MemoryStream(photoBytes);
            
            await cloudBlockBlob.UploadAsync(photoStream, true);
            //logger
            _logger.LogInformation($"successfully upload {newId}");

            var analysisResult = await analyzerService.AnalyzeAsync(photoBytes);
            //comomos db
            var item = new
            {
                id = newId,
                name = request.Name,
                description = request.Description,
                tags = request.Tags,
                AnalysisResult= analysisResult
            };
            items.AddAsync(item);
            _logger.LogInformation($"successfully uploaded item metadata");
            return new OkObjectResult(item);
        }
    }
}
