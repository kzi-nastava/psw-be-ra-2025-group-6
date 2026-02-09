using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Explorer.API.Contracts;
using Microsoft.Extensions.Logging;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[ApiController]
[Route("api/tourist/preferences")]
public class TouristPreferencesController : ControllerBase
{
    private readonly ITouristPreferencesService _preferencesService;
    private readonly ILogger<TouristPreferencesController> _logger;

    public TouristPreferencesController(
        ITouristPreferencesService preferencesService,
        ILogger<TouristPreferencesController> logger)
    {
        _preferencesService = preferencesService;
        _logger = logger;
    }

    [HttpGet("me")]
    public ActionResult<TouristPreferencesDto> GetMine()
    {
        var touristId = User.PersonId();
        var result = _preferencesService.GetByTouristId(touristId);
        _logger.LogDebug("TouristPreferences GET /me: touristId used: {TouristId}, found: {Found}", touristId, result != null);
        if (result == null)
        {
            return Ok(new TouristPreferencesDto
            {
                TouristId = touristId,
                PreferredDifficulty = null,
                WalkRating = 0,
                BikeRating = 0,
                CarRating = 0,
                BoatRating = 0,
                Tags = new List<string>(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                LastSeenRecommendationsAt = null,
                LastNotifiedAt = null
            });
        }

        return Ok(result);
    }

    [HttpPut("me")]
    public ActionResult<TouristPreferencesDto> UpsertMine([FromBody] TouristPreferencesUpsertDto dto)
    {
        var touristId = User.PersonId();
        var result = _preferencesService.Upsert(touristId, dto);
        _logger.LogDebug("TouristPreferences PUT /me: touristId used: {TouristId}", touristId);
        return Ok(result);
    }

    [HttpDelete("me")]
    public IActionResult DeleteMine()
    {
        _preferencesService.Delete(User.PersonId());
        return NoContent();
    }

    [HttpGet]
    public ActionResult<TouristPreferencesDto> GetMineAlias()
    {
        return GetMine();
    }

    [HttpPut]
    public ActionResult<TouristPreferencesDto> UpsertMineAlias([FromBody] TouristPreferencesUpsertDto dto)
    {
        return UpsertMine(dto);
    }

    [HttpDelete]
    public IActionResult DeleteMineAlias()
    {
        return DeleteMine();
    }
}
