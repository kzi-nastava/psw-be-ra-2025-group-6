using Explorer.Payments.API.Public;
using Explorer.Tours.API.Dtos;
using Explorer.API.Contracts;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[ApiController]
[Route("api/tourist/purchases")]
public class PurchasesController : ControllerBase
{
    private readonly ITourPurchaseTokenService _purchaseService;
    private readonly Explorer.Tours.API.Public.Authoring.ITourService _tourService;

    public PurchasesController(ITourPurchaseTokenService purchaseService, Explorer.Tours.API.Public.Authoring.ITourService tourService)
    {
        _purchaseService = purchaseService;
        _tourService = tourService;
    }
    /*
    [HttpGet]
    public ActionResult<List<TourPurchaseTokenDto>> GetMyPurchases()
    {
        var touristId = User.PersonId();
        var tokens = _purchaseService.GetByTouristId(touristId);
        return Ok(tokens);
    }

    */
    [HttpGet("tours")]
    public ActionResult<List<TourDto>> GetMyPurchasedTours()
    {
        if (!User.TryPersonId(out var touristId))
        {
            return Unauthorized(ApiErrorFactory.Create(
                HttpContext,
                ApiErrorCodes.AuthRequired,
                "Login required to view purchases.",
                "User is not recognized (missing tourist profile)."));
        }
        var tokens = _purchaseService.GetByTouristId(touristId);

        var tourIds = tokens.Select(t => t.TourId).Distinct();
        var tours = new List<TourDto>();

        foreach (var id in tourIds)
        {
            try
            {
                var tour = _tourService.Get(id);
                if (tour != null)
                    tours.Add(tour);
            }
            catch
            {
                // ignore missing tours or errors retrieving a specific tour
            }
        }

        return Ok(tours);
    }
}
