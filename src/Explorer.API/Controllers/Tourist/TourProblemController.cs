using Explorer.Tours.API.Public.TourProblem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

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
            var touristId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            problemDto.TouristId = touristId;

            var result = await _tourProblemService.Create(problemDto);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<TourProblemDto>>> GetMyProblems()
        {

            var touristId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var result = await _tourProblemService.GetByTourist(touristId);
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

            var touristId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            await _tourProblemService.Delete(id, touristId);
            return NoContent();
        }
    }
}