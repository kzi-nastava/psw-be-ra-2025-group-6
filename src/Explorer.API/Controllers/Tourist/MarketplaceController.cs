using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/marketplace/tours")]
    [ApiController]
    public class MarketplaceController : ControllerBase
    {
        private readonly ITourService _tourService;

        public MarketplaceController(ITourService tourService)
        {
            _tourService = tourService;
        }

        [HttpGet]
        public ActionResult<PagedResult<TourDto>> GetConfirmedTours([FromQuery] int page, [FromQuery] int pageSize)
        {
            var allTours = _tourService.GetPaged(page, pageSize); 
            
            var confirmedTours = allTours.Results.Where(t => t.Status == TourStatusDto.CONFIRMED).ToList(); 
            
            var pagedResult = new PagedResult<TourDto>(confirmedTours, confirmedTours.Count);

            return Ok(pagedResult);
        }
    }
}