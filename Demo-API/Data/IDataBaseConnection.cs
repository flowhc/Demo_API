using System;
using Demo_API.Types;

namespace Demo_API.Data
{
	public interface IDataBaseConnection
	{
		Task<bool> CreateEntity(DemoEntity demoEntity);
		Task<DemoEntity> ReadEntity(string entityID);
		Task<bool> UpdateEntity(DemoEntity demoEntity);
		Task<bool> DeleteEntity(string entityID);
	}
}

