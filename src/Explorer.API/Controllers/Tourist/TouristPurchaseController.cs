using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/purchases")]
[ApiController]
public class TouristPurchaseController : ControllerBase
{
    private readonly ITourPurchaseTokenService _tourPurchaseTokenService;

    public TouristPurchaseController(ITourPurchaseTokenService tourPurchaseTokenService)
    {
        _tourPurchaseTokenService = tourPurchaseTokenService;
    }

    [HttpGet("tours")]
    public ActionResult<List<TourPurchaseTokenDto>> GetPurchasedTours()
    {
        var touristId = User.PersonId();
        var result = _tourPurchaseTokenService.GetByTouristId(touristId);
        return Ok(result);
    }
}
