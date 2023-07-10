using System.Text;
using Microsoft.AspNetCore.Mvc;
using Demo_API.Data;
using Demo_API.Types;
using Newtonsoft.Json;

namespace Demo_API.Controllers;

[ApiController]
[Route("/")]
public class CRUDController : ControllerBase
{

    private readonly ILogger<CRUDController> _logger;
    private readonly IDataBaseConnection _dataBaseConnection;
    private readonly IDataEnrichment _dataEnrichment;

    public CRUDController(ILogger<CRUDController> logger, IDataBaseConnection dataBaseConnection, IDataEnrichment dataEnrichment)
    {
        _logger = logger;
        _dataBaseConnection = dataBaseConnection;
        _dataEnrichment = dataEnrichment;
    }

    /// <summary>
    /// This Endpoint Creates a new Entity with the values given in the body
    /// </summary>
    /// <returns></returns>
    [HttpPost("Create")]
    public async Task<IActionResult> CreateEntity()
    {
        _logger.LogInformation("Create Entity called");
        string json;
        using (StreamReader reader = new StreamReader(this.Request.Body, Encoding.UTF8))
        {
            json = await reader.ReadToEndAsync();

        }

        _logger.LogInformation(json);

        DemoEntity demoEntity = JsonConvert.DeserializeObject<DemoEntity>(json);

        _logger.LogInformation("funfact: " + demoEntity.FunFact);

        return await _dataBaseConnection.CreateEntity(demoEntity) ? Ok() : BadRequest();

    }

    /// <summary>
    /// This Endpoint returns the requested entity 
    /// </summary>
    /// <param name="entityID">The ID of the desired entity</param>
    /// <returns></returns>
    [HttpGet("Read/{EntityID}")]
    public async Task<IActionResult> ReadEntity(string entityID)
    {
        _logger.LogInformation("Read Entity " + entityID);
        DemoEntity result = await _dataBaseConnection.ReadEntity(entityID);
        string[] funfactWords = result.FunFact.Split(" ");
        string data = await _dataEnrichment.GetMoreData(funfactWords[0]);
        return Ok("[" + JsonConvert.SerializeObject(result) + ",{" + data + "}]");
    }

    /// <summary>
    /// This Endpoint updates the entity given in the body. It creates a new entity if the
    /// one in the body doesnt exist or no ID is specified
    /// </summary>
    /// <returns></returns>
    [HttpPut("Update")]
    public async Task<IActionResult> UpdateEntity()
    {
        _logger.LogInformation("Update Entity");
        string json;
        using (StreamReader reader = new StreamReader(this.Request.Body, Encoding.UTF8))
        {
            json = await reader.ReadToEndAsync();

        }

        _logger.LogInformation(json);

        DemoEntity demoEntity = JsonConvert.DeserializeObject<DemoEntity>(json);
        return await _dataBaseConnection.UpdateEntity(demoEntity)? Ok() : BadRequest();
    }

    /// <summary>
    /// This Enpoint is called to delete a entity
    /// </summary>
    /// <param name="entityID">The ID of the entity to be deleted</param>
    /// <returns></returns>
    [HttpDelete("Delete/{EntityID}")]
    public async Task<IActionResult> DeleteEntity(string entityID)
    {
        _logger.LogInformation("Delete Entity " + entityID);
        bool result = await _dataBaseConnection.DeleteEntity(entityID);
        return result ? Ok() : BadRequest();
    }

    [HttpGet("Data/{query}")]
    public async Task<IActionResult> TestWiki(string query)
    {
        return Ok(await _dataEnrichment.GetMoreData(query));
    }

    [HttpGet("All")]
    public async Task<IActionResult> GetAllEntities()
    {
        List<DemoEntity> results = await _dataBaseConnection.GetAllEntities();
        return Ok(JsonConvert.SerializeObject(results));
    }

}

