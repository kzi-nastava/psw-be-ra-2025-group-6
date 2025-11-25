using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration;

[Authorize(Policy = "administratorPolicy")]
[Route("api/administration/annual-awards")]
[ApiController]
public class AnnualAwardController : ControllerBase
{
    private readonly IAnnualAwardService _annualAwardService;

    public AnnualAwardController(IAnnualAwardService annualAwardService)
    {
        _annualAwardService = annualAwardService;
    }

    [HttpGet]
    public ActionResult<PagedResult<AnnualAwardDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
    {
        return Ok(_annualAwardService.GetPaged(page, pageSize));
    }

    [HttpGet("{id:long}")]
    public ActionResult<AnnualAwardDto> Get(long id)
    {
        return Ok(_annualAwardService.Get(id));
    }

    [HttpPost]
    public ActionResult<AnnualAwardDto> Create([FromBody] AnnualAwardDto annualAwardDto)
    {
        return Ok(_annualAwardService.Create(annualAwardDto));
    }

    [HttpPut("{id:long}")]
    public ActionResult<AnnualAwardDto> Update([FromBody] AnnualAwardDto annualAwardDto)
    {
        return Ok(_annualAwardService.Update(annualAwardDto));
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        _annualAwardService.Delete(id);
        return Ok();
    }
}
