using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.BuildingBlocks.Core.UseCases;
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
            var touristId = User.PersonId();
            var result = _tourPurchaseTokenService.GetByTouristId(touristId);
            return Ok(result);
        }

        [HttpGet("paged")]
        public ActionResult<PagedResult<TourPurchaseTokenDto>> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            var touristId = User.PersonId();
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
