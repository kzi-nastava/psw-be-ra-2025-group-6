using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/tour-problems")]
    [ApiController]
    public class TourProblemController : ControllerBase
    {
        private readonly ITourProblemService _tourProblemService;

        public TourProblemController(ITourProblemService tourProblemService)
        {
            _tourProblemService = tourProblemService;
        }

        [HttpPost]
        public async Task<ActionResult<TourProblemDto>> Create([FromBody] TourProblemDto problemDto)
        {
            problemDto.TouristId = User.PersonId();
            var result = await _tourProblemService.Create(problemDto);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<TourProblemDto>>> GetMyProblems()
        {
            var result = await _tourProblemService.GetByTourist(User.PersonId());
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TourProblemDto>> Update(long id, [FromBody] TourProblemDto problemDto)
        {
            problemDto.Id = id;
            var result = await _tourProblemService.Update(problemDto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            await _tourProblemService.Delete(id, User.PersonId());
            return NoContent();
        }


    }
}