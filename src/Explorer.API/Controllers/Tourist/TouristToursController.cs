using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/tours")]
    [ApiController]
    public class TouristToursController : ControllerBase
    {
        private readonly ITourService _tourService;

        public TouristToursController(ITourService tourService)
        {
            _tourService = tourService;
        }

        [HttpGet("available")] 
        public ActionResult<PagedResult<TourDto>> GetAvailableToursPages([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var touristId = User.PersonId();
            var result = _tourService.GetAvailableForTouristPaged(touristId, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public ActionResult<TourDto> Get(long id)
        {
            return Ok(_tourService.Get(id));
        }
    }
}