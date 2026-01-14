using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers;

[Authorize(Policy = "touristPolicy")]
[Route("api/tour-planner")]
[ApiController]
public class TourPlannerController : ControllerBase
{
    private readonly ITourPlannerService _tourPlannerService;

    public TourPlannerController(ITourPlannerService tourPlannerService)
    {
        _tourPlannerService = tourPlannerService;
    }

    private long CurrentUserId() => User.PersonId();

    [HttpGet]
    public ActionResult<List<TourPlannerDto>> GetAllForUser()
    {
        var result = _tourPlannerService.GetAllByUserId(CurrentUserId());
        return Ok(result);
    }

    [HttpGet("paged")]
    public ActionResult<PagedResult<TourPlannerDto>> GetPagedForUser([FromQuery] int page, [FromQuery] int pageSize)
    {
        var result = _tourPlannerService.GetByUserId(CurrentUserId(), page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public ActionResult<TourPlannerDto> GetById(long id)
    {
        var planner = _tourPlannerService.GetById(id);
        if (planner.UserId != CurrentUserId())
        {
            return Forbid("You can not access someone else's planner item.");
        }
        return Ok(planner);
    }

    [HttpPost]
    public ActionResult<TourPlannerDto> Create([FromBody] TourPlannerCreateDto dto)
    {
        var result = _tourPlannerService.Create(CurrentUserId(), dto);
        return Ok(result);
    }

    [HttpPut("{id:long}")]
    public ActionResult<TourPlannerDto> Update(long id, [FromBody] TourPlannerUpdateDto dto)
    {
        var existing = _tourPlannerService.GetById(id);
        if (existing.UserId != CurrentUserId())
        {
            return Forbid("You can not update someone else's planner item.");
        }

        var result = _tourPlannerService.Update(id, CurrentUserId(), dto);
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        var existing = _tourPlannerService.GetById(id);
        if (existing.UserId != CurrentUserId())
        {
            return Forbid("You can not delete someone else's planner item.");
        }

        _tourPlannerService.Delete(id);
        return NoContent();
    }
}
