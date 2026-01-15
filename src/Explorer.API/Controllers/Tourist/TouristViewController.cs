using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.UseCases.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [ApiController]
    [Route("api/tourist/tours")]
    public class TouristViewController : ControllerBase
    {
        private readonly ITouristViewService _touristService;
        private readonly ITourService _tourService;

        public TouristViewController(ITouristViewService touristService, ITourService tourService)
        {
            _touristService = touristService;
            _tourService = tourService;
        }

        [HttpGet("published")]
        public ActionResult<List<TourDto>> GetPublishedTours()
        {
            var touristId = User.PersonId();
            var tours = _tourService.GetAvailableForTourist(touristId);
            return Ok(tours);
        }

        [HttpGet("available")]
        public ActionResult<PagedResult<TourDto>> GetAvailableTours([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var touristId = User.PersonId();
            var tours = _tourService.GetAvailableForTouristPaged(touristId, page, pageSize);
            return Ok(tours);
        }
    }
}
