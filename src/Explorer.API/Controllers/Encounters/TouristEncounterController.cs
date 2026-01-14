using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Encounters;

[Authorize(Policy = "touristPolicy")]
[ApiController]
[Route("api/tourist/encounters")]
public class TouristEncounterController : ControllerBase
{
    private readonly ITouristEncounterService _touristEncounterService;

    public TouristEncounterController(ITouristEncounterService touristEncounterService)
    {
        _touristEncounterService = touristEncounterService;
    }

    //Pogledati   private long GetTouristId() =>PersonId();
    private long GetTouristId() =>
    long.Parse(User.FindFirst("id")!.Value);

    /// <summary>
    /// Get tourist's XP profile (current XP, level, progress)
    /// </summary>
    [HttpGet("profile")]
    public ActionResult<TouristXpProfileDto> GetProfile()
    {
        var touristId = GetTouristId();
        var profile = _touristEncounterService.GetOrCreateProfile(touristId);
        return Ok(profile);
    }

    /// <summary>
    /// Get all active challenges and which ones the tourist has completed
    /// </summary>
    [HttpGet("active")]
    public ActionResult<ActiveChallengesResponseDto> GetActiveChallenges()
    {
        var touristId = GetTouristId();
        var result = _touristEncounterService.GetActiveChallenges(touristId);
        return Ok(result);
    }

    /// <summary>
    /// Complete a Misc Encounter (self-check)
    /// </summary>
    [HttpPost("complete")]
    public ActionResult<CompleteEncounterResponseDto> CompleteEncounter([FromBody] CompleteEncounterRequestDto request)
    {
        var touristId = GetTouristId();
        var result = _touristEncounterService.CompleteEncounter(touristId, request);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Get all encounters completed by the tourist
    /// </summary>
    [HttpGet("completed")]
    public ActionResult<List<EncounterCompletionDto>> GetCompletedEncounters()
    {
        var touristId = GetTouristId();
        var completions = _touristEncounterService.GetCompletedEncounters(touristId);
        return Ok(completions);
    }
}
