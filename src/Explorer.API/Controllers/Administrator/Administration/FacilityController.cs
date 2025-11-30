using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration;


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
    [Authorize(Roles = "administrator, tourist")]
    public ActionResult<PagedResult<FacilityDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
    {
        return Ok(_facilityService.GetPaged(page, pageSize));
    }

    [HttpPost]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult<FacilityDto> Create([FromBody] FacilityDto facility)
    {
        return Ok(_facilityService.Create(facility));
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult<FacilityDto> Update([FromBody] FacilityDto facility)
    {
        return Ok(_facilityService.Update(facility));
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult Delete(long id)
    {
        _facilityService.Delete(id);
        return Ok();
    }
}

