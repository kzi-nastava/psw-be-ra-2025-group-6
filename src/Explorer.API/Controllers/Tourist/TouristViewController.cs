<<<<<<< Updated upstream
﻿using Microsoft.AspNetCore.Mvc;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.UseCases.Tourist;
=======
﻿using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.UseCases.Tourist;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
>>>>>>> Stashed changes
using System.Collections.Generic;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Public;



using Microsoft.AspNetCore.Authorization;



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
    }
}
