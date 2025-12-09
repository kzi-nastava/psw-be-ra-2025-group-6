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

    /// <summary>
    /// Proverava napredak turiste na aktivnoj turi
    /// </summary>
    /// <param name="id">ID TourExecution sesije</param>
    /// <param name="dto">Trenutne GPS koordinate turiste</param>
    /// <returns>Informacije o napretku i eventualnoj completiranoj ta?ci</returns>
    [HttpPost("{id}/check-progress")]
    public ActionResult<ProgressResponseDto> CheckProgress(long id, [FromBody] TrackPointDto dto)
    {
        var touristId = GetTouristId();
        var result = _executionService.CheckProgress(id, dto, touristId);
        return Ok(result);
    }

    /// <summary>
    /// Vra?a listu otklju?anih tajni za konkretnu sesiju
    /// </summary>
    /// <param name="id">ID TourExecution sesije</param>
    /// <returns>Lista otklju?anih tajni</returns>
    [HttpGet("{id}/unlocked-secrets")]
    public ActionResult<UnlockedSecretsDto> GetUnlockedSecrets(long id)
    {
        var touristId = GetTouristId();
        var result = _executionService.GetUnlockedSecrets(id, touristId);
        return Ok(result);
    }
}
