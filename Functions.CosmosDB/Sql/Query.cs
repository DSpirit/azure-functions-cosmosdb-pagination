using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Functions.CosmosDB.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace Functions.CosmosDB.Sql
{
    /// <summary>
    /// The Query Class.
    /// </summary>
    public static class Query
    {
        /// <summary>
        /// The response continuation token size limit in kb.
        /// </summary>
        private const int ResponseContinuationTokenLimit = 1;


        /// <summary>
        /// Enable cross partition query.
        /// </summary>
        private const bool EnableCrossPartitionQuery = true;

        /// <summary>
        /// The default page size.
        /// </summary>
        private const int DefaultPageSize = 100;

        /// <summary>
        /// Runs the specified req.
        /// </summary>
        /// <param name="req">The req.</param>
        /// <param name="database">The database.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="client">The client.</param>
        /// <param name="log">The log.</param>
        /// <param name="context">The context.</param>
        /// <returns>The result.</returns>
        [FunctionName(nameof(PagedQuery))]
        public static async Task<IActionResult> PagedQuery(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "{database}/{collection}")] HttpRequest req,
            string database,
            string collection,
            [CosmosDB("{database}", "{collection}", ConnectionStringSetting = "CosmosDBConnection")] IDocumentClient client,
            ILogger log,
            ExecutionContext context)
        {
            var functionName = context.FunctionName;
            var queryString = req.GetQueryParameterDictionary();
            var headers = req.GetTypedHeaders();
            var pageSize = int.Parse(queryString.ContainsKey("pageSize") ? queryString["pageSize"] : DefaultPageSize.ToString());
            var continuation = queryString.ContainsKey("continuation") ? queryString["continuation"] : null;
            
            var results = new List<dynamic>();

            var cosmosQuery = JObject.Parse(queryString["q"]);
            var querySpec = new SqlQuerySpec(cosmosQuery["QueryText"]?.ToString(), JsonConvert.DeserializeObject<SqlParameterCollection>(cosmosQuery["Parameters"]?.ToString()));

            try
            {
                Uri collectionUri = UriFactory.CreateDocumentCollectionUri(database, collection);

                if (string.IsNullOrEmpty(querySpec.QueryText))
                {
                    throw new ArgumentNullException("Please provide a query!");
                }

                var response = new PagedResults();
                
                // Loop through partition key ranges
                foreach (var pkRange in await client.ReadPartitionKeyRangeFeedAsync(collectionUri))
                {
                    try
                    {
                        var options = new FeedOptions() { 
                            PartitionKeyRangeId = pkRange.Id, 
                            RequestContinuation = continuation, 
                            MaxItemCount = pageSize - results.Count, 
                            ResponseContinuationTokenLimitInKb = ResponseContinuationTokenLimit, 
                            EnableCrossPartitionQuery = EnableCrossPartitionQuery 
                        };

                        using (var query = client
                        .CreateDocumentQuery<dynamic>(collectionUri, querySpec, options)
                        .AsDocumentQuery())
                        {
                            while (query.HasMoreResults && results.Count < pageSize)
                            {
                                // Get results
                                FeedResponse<dynamic> recordSet = await query.ExecuteNextAsync();
                                foreach (var record in recordSet)
                                {
                                    results.Add(record);
                                }

                                // Create Response Object
                                response.Pagination.ContinuationToken = recordSet.ResponseContinuation;

                                // Set Links
                                var referrer = headers.Referer ?? new Uri(req.GetEncodedUrl());
                                response.Links.Self = referrer.OriginalString;
                                
                                NameValueCollection qc = HttpUtility.ParseQueryString(referrer.Query, Encoding.UTF8);
                                qc.Remove("continuation");

                                if (!string.IsNullOrEmpty(recordSet.ResponseContinuation))
                                {
                                    qc.Add("continuation", recordSet.ResponseContinuation);
                                    response.Links.Next = $"{req.Scheme}://{req.Host}{req.Path}?{string.Join("&", qc.AllKeys.Select(k => string.Format("{0}={1}", k, Uri.EscapeDataString(qc[k]))))}";
                                }
                                else
                                {
                                    response.Links.Next = null;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        if (e.Message.Contains("Microsoft.Azure.Documents.Query.CompositeContinuationToken"))
                        {   
                            log.LogDebug("Skipping partition key range.");
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                response.Data = results;

                return new OkObjectResult(response);
            }
            catch (Exception e)
            {   
                log.LogError($"{functionName} failed", e);

                var response = new
                {
                    e.Message,
                };

                return new BadRequestObjectResult(response);
            }
        }
    }
}