using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[ApiController]
[Route("api/tourist/recommendations")]
public class TouristRecommendationsController : ControllerBase
{
    private readonly ITourRecommendationService _recommendationService;
    private readonly ITouristPreferencesService _preferencesService;

    public TouristRecommendationsController(
        ITourRecommendationService recommendationService,
        ITouristPreferencesService preferencesService)
    {
        _recommendationService = recommendationService;
        _preferencesService = preferencesService;
    }

    [HttpGet]
    public ActionResult<TourRecommendationsDto> Get([FromQuery] int? limit = 6)
    {
        var resolvedLimit = limit.HasValue && limit.Value > 0 ? limit.Value : 6;
        var summary = _recommendationService.GetRecommendationSummary(User.PersonId(), resolvedLimit);
        var response = new TourRecommendationsDto
        {
            TourIds = summary.TourIds,
            NewTourIds = summary.NewTourIds
        };
        return Ok(response);
    }

    [HttpPost("ack")]
    public IActionResult Acknowledge()
    {
        var updated = _preferencesService.MarkRecommendationsSeen(User.PersonId(), DateTime.UtcNow);
        if (updated == null) return NotFound();
        return NoContent();
    }
}
