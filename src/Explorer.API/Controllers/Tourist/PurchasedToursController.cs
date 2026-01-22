using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.API.Contracts;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/purchased-tours")]
    public class PurchasedToursController : ControllerBase
    {
        private readonly ITourPurchaseTokenService _tourPurchaseTokenService;

        public PurchasedToursController(ITourPurchaseTokenService tourPurchaseTokenService)
        {
            _tourPurchaseTokenService = tourPurchaseTokenService;
        }

        [HttpGet]
        public ActionResult<List<TourPurchaseTokenDto>> Get()
        {
            if (!User.TryPersonId(out var touristId))
            {
                return Unauthorized(ApiErrorFactory.Create(
                    HttpContext,
                    ApiErrorCodes.AuthRequired,
                    "Login required to view purchases.",
                    "User is not recognized (missing tourist profile)."));
            }
            var result = _tourPurchaseTokenService.GetByTouristId(touristId);
            return Ok(result);
        }

        [HttpGet("paged")]
        public ActionResult<PagedResult<TourPurchaseTokenDto>> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            if (!User.TryPersonId(out var touristId))
            {
                return Unauthorized(ApiErrorFactory.Create(
                    HttpContext,
                    ApiErrorCodes.AuthRequired,
                    "Login required to view purchases.",
                    "User is not recognized (missing tourist profile)."));
            }
            var tokens = _tourPurchaseTokenService.GetByTouristId(touristId);
            var totalCount = tokens.Count;
            var items = tokens
                .OrderByDescending(t => t.PurchaseDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return Ok(new PagedResult<TourPurchaseTokenDto>(items, totalCount));
        }
    }
}
