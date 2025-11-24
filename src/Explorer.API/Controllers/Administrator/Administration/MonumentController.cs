using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration;

[Authorize(Policy = "administratorPolicy")]
[Route("api/administration/monuments")]
[ApiController]
public class MonumentController : ControllerBase
{
    private readonly IMonumentService _monumentService;

    public MonumentController(IMonumentService monumentService)
    {
        _monumentService = monumentService;
    }

    [HttpGet]
    public ActionResult<PagedResult<MonumentDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
    {
        return Ok(_monumentService.GetPaged(page, pageSize));
    }

    [HttpGet("{id:long}")]
    public ActionResult<MonumentDto> Get(long id)
    {
        return Ok(_monumentService.Get(id));
    }

    [HttpPost]
    public ActionResult<MonumentDto> Create([FromBody] MonumentDto monument)
    {
        return Ok(_monumentService.Create(monument));
    }

    [HttpPut("{id:long}")]
    public ActionResult<MonumentDto> Update([FromBody] MonumentDto monument)
    {
        return Ok(_monumentService.Update(monument));
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        _monumentService.Delete(id);
        return Ok();
    }
}
