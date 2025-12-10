using Microsoft.AspNetCore.Mvc;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.UseCases.Tourist;
using System.Collections.Generic;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Public;

namespace Explorer.Tours.API.Controllers.Tourist
{
    [ApiController]
    [Route("api/tourist/tours")]
    public class TouristViewController : ControllerBase
    {
        private readonly ITouristViewService _touristService;

        public TouristViewController(ITouristViewService touristService)
        {
            _touristService = touristService;
        }

        [HttpGet("published")]
        public ActionResult<List<TouristTourDto>> GetPublishedTours()
        {
            try
            {
                var tours = _touristService.GetPublishedTours();
                return Ok(tours);
            }
            catch (Exception ex)
            {
                // Vrati KOMPLETNU grešku kao response
                return StatusCode(500, new
                {
                    message = ex.Message,
                    innerException = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace,
                    type = ex.GetType().Name
                });
            }
        }
    }
}
