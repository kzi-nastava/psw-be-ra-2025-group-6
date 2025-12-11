using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Shopping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/tours")]
[ApiController]
public class TourMarketplaceController : ControllerBase
{
    private readonly ITourShoppingService _tourShoppingService;

    public TourMarketplaceController(ITourShoppingService tourShoppingService)
    {
        _tourShoppingService = tourShoppingService;
    }

    [HttpGet("search/distance")]
    public ActionResult<List<TourDto>> SearchByDistance(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusInKm)
    {
        if (latitude < -90 || latitude > 90) return BadRequest("Latitude must be between -90 and 90.");
        if (longitude < -180 || longitude > 180) return BadRequest("Longitude must be between -180 and 180.");
        if (radiusInKm <= 0) return BadRequest("Radius must be greater than 0.");

        var result = _tourShoppingService.SearchByDistance(latitude, longitude, radiusInKm);
        return Ok(result);
    }
}
