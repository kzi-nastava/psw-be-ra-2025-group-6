using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration;

[Authorize(Policy = "administratorPolicy")]
[Route("api/administration/facility")]
[ApiController]
public class FacilityController : ControllerBase
{
    private readonly IFacilityService _facilityService;

    public FacilityController(IFacilityService facilityService)
    {
        _facilityService = facilityService;
    }

    [HttpGet]
    public ActionResult<PagedResult<FacilityDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
    {
        return Ok(_facilityService.GetPaged(page, pageSize));
    }

    [HttpPost]
    public ActionResult<FacilityDto> Create([FromBody] FacilityDto facility)
    {
        return Ok(_facilityService.Create(facility));
    }

    [HttpPut("{id:long}")]
    public ActionResult<FacilityDto> Update([FromBody] FacilityDto facility)
    {
        return Ok(_facilityService.Update(facility));
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        _facilityService.Delete(id);
        return Ok();
    }
}

