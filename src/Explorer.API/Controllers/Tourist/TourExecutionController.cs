using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Execution;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tours/execution")]
[ApiController]
public class TourExecutionController : ControllerBase
{
    private readonly ITourExecutionService _executionService;

    public TourExecutionController(ITourExecutionService executionService)
    {
        _executionService = executionService;
    }

    private long GetTouristId() => User.PersonId();

    [HttpPost("start")]
    public ActionResult<TourExecutionStartResultDto> Start([FromBody] TourExecutionStartDto dto)
    {
        var touristId = GetTouristId();

        var result = _executionService.StartExecution(dto, touristId);
        return CreatedAtAction(nameof(Start), result);
    }

    [HttpGet("active")]
    public ActionResult<TourExecutionStartResultDto> GetActive([FromQuery] long? tourId)
    {
        var touristId = GetTouristId();
        var result = _executionService.GetActiveExecution(touristId, tourId);
        if (result == null) return NotFound(new { title = "No active session", detail = "Nema aktivne sesije" });
        return Ok(result);
    }

    [HttpPut("{executionId}/complete")]
    public ActionResult<TourExecutionResultDto> Complete(long executionId)
    {
        var touristId = GetTouristId();
        var result = _executionService.CompleteExecution(executionId, touristId);
        return Ok(result);
    }

    [HttpPut("{executionId}/abandon")]
    public ActionResult<TourExecutionResultDto> Abandon(long executionId)
    {
        var touristId = GetTouristId();
        var result = _executionService.AbandonExecution(executionId, touristId);
        return Ok(result);
    }
}
