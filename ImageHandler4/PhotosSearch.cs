using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ImageHandler.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace ImageHandler
{
    public static class PhotosSearch
    {
        [FunctionName("PhotosSearch")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            //input binding to read data. 
            [CosmosDB("photos", "metadata", Connection = Literals.CosmosDBConnection)] CosmosClient cosmosClient,
            ILogger log)
        {
            log.LogInformation("searching...");
            var searchTerm = req.Query["Searchitem"];
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new NotFoundResult();
            }
            //var collectionUri = UriFactory.CreateDocumentCollectionUri("photos", "metadata");
            var container = cosmosClient.GetContainer("photos", "metadata");
            /*//storongly typed query
            var query = client.CreateDocumentQuery<PhotoUploadModel>(collectionUri,
                   new FeedOptions() { EnableCrossPartitionQuery = true })
              .Where(p => p.Description.Contains(searchItem))
             .AsDocumentQuery();*/
            // Use LINQ for cleaner query definition
            var query = container.GetItemLinqQueryable<PhotoUploadModel>()
              .Where(p => p.Description.Contains(searchTerm));


            var results = new List<dynamic>();
            var iterator = query.ToFeedIterator();

            while ( iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response);
            }
            return new OkObjectResult(results);
        }
    }
}
