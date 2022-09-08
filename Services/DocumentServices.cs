using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Documents;

namespace Datacom.Envirohack
{
    public static class DocumentServices
    {
        public static async Task<(DocumentResponse<T> response, HttpStatusCode? statusCode, string message)> GetDocument<T>(DocumentClient client, string collection, string id){
            // Get the cosmosdb document using the session state
            var tokenDocLink = $"dbs/{Constants.DbMain}/colls/{collection}/docs/{id}";
            var requestOptions = new RequestOptions { PartitionKey = new PartitionKey(id) };
            DocumentResponse<T> tokenDoc;
            
            try
            {
                tokenDoc = await client.ReadDocumentAsync<T>(tokenDocLink, requestOptions);
            }
            catch (DocumentClientException e)
            {
                return (null, e.StatusCode, e.Message);
            }

            return (tokenDoc, HttpStatusCode.OK, null);
        }

        /// <summary> Upserts the document in CosmosDB </summary>
        public static async Task<HttpStatusCode?> UpsertDocument<T>(DocumentClient client, string collection, IDocument document){
            try
            {
                var requestOptions = new RequestOptions { PartitionKey = new PartitionKey(document.Id) };
                var collectionUri = UriFactory.CreateDocumentCollectionUri(Constants.DbMain, collection);
                await client.UpsertDocumentAsync(collectionUri, document, requestOptions);
            }
            catch (DocumentClientException e)
            {
                return e.StatusCode;
            }

            return HttpStatusCode.OK;
        }

        public static async Task<HttpStatusCode?> DeleteDocument(DocumentClient client, string collection, string id){
            try
            {
                var requestOptions = new RequestOptions { PartitionKey = new PartitionKey(id) };
                var documentUri = UriFactory.CreateDocumentUri(Constants.DbMain, collection, id);
                await client.DeleteDocumentAsync(documentUri, requestOptions);
            }
            catch (DocumentClientException e)
            {
                var message = e.Message;
                return e.StatusCode;
            }

            return HttpStatusCode.OK;
        }
        
        public static async Task<List<T>> GetDocuments<T>(DocumentClient client, string collection, IEnumerable<string> ids)
        {
            var encapsulatedIds = ids.Select(id => $"'{id}'").ToList();
            return await GetDocumentsByQuery<T>(client, collection, $"SELECT * FROM c WHERE c.id IN ({string.Join(",", encapsulatedIds)})");
        }

        public static async Task<List<T>> GetAllDocuments<T>(DocumentClient client, string collection)
        {
            var results = new List<T>();
            var collectionUri = UriFactory.CreateDocumentCollectionUri(Constants.DbMain, collection);
            using (var queryable = client.CreateDocumentQuery<T>(
                collectionUri,
                new FeedOptions { MaxItemCount = 10 })
                .AsDocumentQuery())
            {
                while (queryable.HasMoreResults) 
                {
                    foreach (T result in await queryable.ExecuteNextAsync())
                    {
                        results.Add(result);
                    }
                }
            }

            return results;
        }

        /// <summary> Gets the documents from CosmosDB using a query string like `$"SELECT * FROM c WHERE c.orgId = '{orgId}'`</summary>
        public static async Task<List<T>> GetDocumentsByQuery<T>(DocumentClient client, string collection, string queryString)
        {
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(Constants.DbMain, collection);
            
            var query = client.CreateDocumentQuery(collectionUri, queryString, new FeedOptions { MaxItemCount = 10, EnableCrossPartitionQuery=true })
                .AsDocumentQuery();

            var results = new List<T>();
            while (query.HasMoreResults)
            {
                foreach (T result in await query.ExecuteNextAsync())
                {
                    results.Add(result);
                }
            }

            return results;
        }

        private static FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true };
    }
}
