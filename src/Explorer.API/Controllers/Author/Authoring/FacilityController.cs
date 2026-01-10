using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author.Authoring;

[Authorize(Policy = "authorPolicy")]
[Route("api/author/facilities")]
[ApiController]
public class FacilityController : ControllerBase
{
    private readonly IFacilityService _facilityService;

    public FacilityController(IFacilityService facilityService)
    {
        _facilityService = facilityService;
    }

    [HttpGet]
    public ActionResult<List<FacilityDto>> GetAll()
    {
        return Ok(_facilityService.GetPaged(1, int.MaxValue).Results);
    }

    [HttpGet("public")]
    public ActionResult<List<FacilityDto>> GetPublic()
    {
        var all = _facilityService.GetPaged(1, int.MaxValue).Results;
        return Ok(all.Where(f => f.IsPublic).ToList());
    }

    [HttpGet("{id:long}")]
    public ActionResult<FacilityDto> Get(long id)
    {
        return Ok(_facilityService.Get(id));
    }

    [HttpPost]
    public ActionResult<FacilityDto> Create([FromBody] FacilityDto facility)
    {
        // Ensure created facility is private
        facility.IsPublic = false;
        facility.PublicRequestId = null;
        var userId = User.PersonId();
        // Optionally set author in DTO if exists
        var result = _facilityService.Create(facility);
        return Ok(result);
    }

    [HttpPut("{id:long}")]
    public ActionResult<FacilityDto> Update(long id, [FromBody] FacilityDto facility)
    {
        var existing = _facilityService.Get(id);
        // Only author of facility should edit - but Facility does not track author
        facility.Id = id;
        var result = _facilityService.Update(facility);
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        _facilityService.Delete(id);
        return Ok();
    }
}
