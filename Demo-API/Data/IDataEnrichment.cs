using System;
namespace Demo_API.Data
{
	public interface IDataEnrichment
	{
		public Task<string> GetMoreData(string query);
	}
}

