using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Explorer.API.Controllers.Author
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/author/tour-problems")]
    [ApiController]
    public class TourProblemAuthorController : ControllerBase
    {
        private readonly ITourProblemService _tourProblemService;

        public TourProblemAuthorController(ITourProblemService tourProblemService)
        {
            _tourProblemService = tourProblemService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TourProblemDto>>> GetMyProblems()
        {
            var result = await _tourProblemService.GetByAuthor(User.PersonId());
            return Ok(result);
        }
    }
}
