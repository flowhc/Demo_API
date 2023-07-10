using System;
using Demo_API.Data;
namespace Demo_API.Wikipedia
{
	public class WikipediaDataEnrichment : IDataEnrichment
	{
        HttpClient client;

		public WikipediaDataEnrichment(string uri)
		{
            client = new HttpClient();
            client.BaseAddress = new Uri(uri);
		}

        public async Task<string> GetMoreData(string query)
        {
            string response = await client.GetStringAsync("?action=opensearch&namespace=0&search=" + query + "&limit=5&format=json");
            Console.WriteLine("Response: " + response);
            return response;
        }
    }
}

