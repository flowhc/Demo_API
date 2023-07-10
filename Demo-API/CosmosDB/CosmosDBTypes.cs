using System;
using Demo_API.Types;
using Microsoft.WindowsAzure.Storage.Table;

namespace Demo_API.CosmosDB
{

    /// <summary>
    /// A entity which is compatible with the CosmosDB 
    /// </summary>
    public class CosmosDBDemoEntity : TableEntity
    {
        public string FunFact { get; set; }

        public CosmosDBDemoEntity() { }


        /// <summary>
        /// Constructor which tage a Demo Entity, to create a new CosmosDB Entity.
        /// When you dont specify a RowKey, this constructor will generate one 
        /// </summary>
        /// <param name="demoEntity"></param>
        public CosmosDBDemoEntity(DemoEntity demoEntity)
        {
            this.PartitionKey = "0";
            this.RowKey = demoEntity.RowKey == null ? Guid.NewGuid().ToString("N") : demoEntity.RowKey;
            this.FunFact = demoEntity.FunFact;
        }

        /// <summary>
        /// Converts a CosmosDBEntity to a DemoEntity 
        /// </summary>
        /// <returns></returns>
        public DemoEntity CosmosDBEntityToDemoEntity()
        {
            return new DemoEntity
            {
                PartitionKey = this.PartitionKey,
                RowKey = this.RowKey,
                FunFact = this.FunFact
            };
        }
    }
}


