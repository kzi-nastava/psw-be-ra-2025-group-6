using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Marketplace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/tours")]
[ApiController]
public class TourMarketplaceController : ControllerBase
{
    private readonly ITourMarketplaceService _tourMarketplaceService;

    public TourMarketplaceController(ITourMarketplaceService tourMarketplaceService)
    {
        _tourMarketplaceService = tourMarketplaceService;
    }

    [HttpPost("search-by-distance")]
    public ActionResult<List<TourSummaryDto>> SearchByDistance([FromBody] TourSearchByDistanceRequestDto request)
    {
        try
        {
            var result = _tourMarketplaceService.SearchByDistance(request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { title = "Validation Error", detail = ex.Message });
        }
    }
}
