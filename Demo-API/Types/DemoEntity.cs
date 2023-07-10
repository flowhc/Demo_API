using System;
namespace Demo_API.Types
{
	public class DemoEntity
	{
        public string PartitionKey;
        public string RowKey;
        public string FunFact;

        public DemoEntity()
		{
		}

        public DemoEntity(string funfact)
        {
            this.FunFact = funfact;
        }
    }
}

