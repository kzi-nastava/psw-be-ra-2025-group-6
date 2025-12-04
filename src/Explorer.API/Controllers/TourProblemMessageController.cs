using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Explorer.API.Controllers
{
    [Authorize(Policy = "registeredUserPolicy")]
    [Route("api/tour-problem-messages")]
    [ApiController]
    public class TourProblemMessageController : ControllerBase
    {
        private readonly ITourProblemMessageService _tourProblemMessageService;

        public TourProblemMessageController(ITourProblemMessageService tourProblemMessageService)
                {
            _tourProblemMessageService = tourProblemMessageService;
        }

        [HttpPost]
        public async Task<ActionResult<TourProblemMessageDto>> Create([FromBody] TourProblemMessageDto messageDto)
        {
            messageDto.SenderId = User.PersonId();
            var result = await _tourProblemMessageService.Create(messageDto);
            return Ok(result);
        }

        [HttpGet("{problemId:long}")]
        public async Task<ActionResult<PagedResult<TourProblemMessageDto>>> GetForProblem(long problemId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _tourProblemMessageService.GetForProblem(problemId, page, pageSize);
                return Ok(result);
            }
            catch (Exception e)
            {
                return new ObjectResult(e.ToString()) { StatusCode = 500 };
            }
        }
    }
}
