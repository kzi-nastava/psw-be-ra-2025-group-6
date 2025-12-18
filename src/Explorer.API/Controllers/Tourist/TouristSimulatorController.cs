using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/simulator")]
[ApiController]
public class TouristSimulatorController : ControllerBase
{
    private readonly ITouristPositionService _positionService;

    public TouristSimulatorController(ITouristPositionService positionService)
    {
        _positionService = positionService;
    }

    private long GetTouristId() => User.PersonId();

    [HttpPost("position")]
    public ActionResult<TouristPositionDto> SavePosition([FromBody] TouristPositionDto dto)
    {
        if (dto.TouristId != GetTouristId()) return Forbid();
        var result = _positionService.CreateOrUpdate(dto);
        return CreatedAtAction(nameof(SavePosition), result);
    }

    [HttpGet("position")]
    public ActionResult<TouristPositionDto> GetPosition()
    {
        var touristId = GetTouristId();
        var result = _positionService.GetByTouristId(touristId);
        if (result == null) return NotFound();
        return Ok(result);
    }
}
