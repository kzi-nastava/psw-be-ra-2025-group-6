using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers;

[Authorize(Policy = "touristPolicy")]
[ApiController]
[Route("api/tours")]
public class TourRecommendationsController : ControllerBase
{
    private readonly ITourRecommendationService _recommendationService;

    public TourRecommendationsController(ITourRecommendationService recommendationService)
    {
        _recommendationService = recommendationService;
    }

    [HttpGet("recommended")]
    public ActionResult<List<TourDto>> GetRecommended([FromQuery] int limit = 6)
    {
        if (limit <= 0) limit = 6;
        var result = _recommendationService.GetRecommended(User.UserId(), limit);
        return Ok(result);
    }
}
