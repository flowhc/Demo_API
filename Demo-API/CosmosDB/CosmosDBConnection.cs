using System;
using System.Collections.Concurrent;
using Demo_API.Data;
using Demo_API.Types;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Demo_API.CosmosDB
{
    /// <summary>
    /// Implementation of the IDataBaseConnection interface for the CosmosDB Tables API
    /// </summary>
    public class CosmosDBConnection : IDataBaseConnection
    {

        private readonly CloudTable entityTable;

        ManualResetEventSlim initialized = new ManualResetEventSlim();
        ManualResetEventSlim initializing = new ManualResetEventSlim();


        private async Task InitializeAsync()
        {
            if (!this.initialized.Wait(0))
            {
                if (!this.initializing.Wait(0))
                {
                    this.initializing.Set();
                    await this.entityTable.CreateIfNotExistsAsync();

                    this.initialized.Set();
                }

                this.initialized.Wait();
            }
        }

        public CosmosDBConnection(string storageConnectionString)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            this.entityTable = tableClient.GetTableReference("Demo_API");
        }

        /// <summary>
        /// This Methode executes queries in the CosmosDB
        /// </summary>
        /// <param name="tableQuery">The query</param>
        /// <returns></returns>
        private async Task<List<CosmosDBDemoEntity>> GetFromAnchorCache(TableQuery<CosmosDBDemoEntity> tableQuery)
        {
            List<CosmosDBDemoEntity> results = new List<CosmosDBDemoEntity>();
            TableQuerySegment<CosmosDBDemoEntity> previousSegment = null;
            while (previousSegment == null || previousSegment.ContinuationToken != null)
            {
                TableQuerySegment<CosmosDBDemoEntity> currentSegment = await this.entityTable.
                    ExecuteQuerySegmentedAsync<CosmosDBDemoEntity>(tableQuery, previousSegment?.ContinuationToken);
                previousSegment = currentSegment;
                results.AddRange(previousSegment.Results);
            }
            return results;
        }

        /// <summary>
        /// Create a entity
        /// </summary>
        /// <param name="demoEntity">The entity which should be created in the database</param>
        /// <returns></returns>
        public async Task<bool> CreateEntity(DemoEntity demoEntity)
        {
            try
            {
                CosmosDBDemoEntity cosmosDBDemoEntity = new CosmosDBDemoEntity(demoEntity);
                await this.entityTable.ExecuteAsync(TableOperation.Insert(cosmosDBDemoEntity));

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Delete a entity
        /// </summary>
        /// <param name="entityID">Specify the entity which should be deleted</param>
        /// <returns></returns>
        public async Task<bool> DeleteEntity(string entityID)
        {
            try
            {
                await this.entityTable.ExecuteAsync(TableOperation.Delete(new CosmosDBDemoEntity()
                {
                    ETag = "*",
                    PartitionKey = "0",
                    RowKey = entityID
                }));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get a entity from the CosmosDB
        /// </summary>
        /// <param name="entityID">ID of the requested entity</param>
        /// <returns></returns>
        public async Task<DemoEntity> ReadEntity(string entityID)
        {
            List<CosmosDBDemoEntity> results = await GetFromAnchorCache(new TableQuery<CosmosDBDemoEntity>()
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, entityID)));
            return results != null ? results[0].CosmosDBEntityToDemoEntity() : new DemoEntity();
        }

        /// <summary>
        /// Update or create a entity
        /// </summary>
        /// <param name="demoEntity">A entity which should be updated (if you dont include a entityID, a new entity will be created)</param>
        /// <returns></returns>
        public async Task<bool> UpdateEntity(DemoEntity demoEntity)
        {
            try
            {
                await this.entityTable.ExecuteAsync(TableOperation.InsertOrReplace(new CosmosDBDemoEntity(demoEntity)));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

