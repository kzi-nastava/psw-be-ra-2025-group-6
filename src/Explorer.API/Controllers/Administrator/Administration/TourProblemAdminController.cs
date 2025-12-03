using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/tour-problems")]
    [Authorize(Policy = "AdministratorPolicy")]
    public class TourProblemAdminController : ControllerBase
    {
        private readonly ITourProblemService _tourProblemService;

        public TourProblemAdminController(ITourProblemService tourProblemService)
        {
            _tourProblemService = tourProblemService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TourProblemDto>>> GetAll()
        {
            var result = await _tourProblemService.GetAll();
            return Ok(result);
        }

        [HttpPost("{id}/deadline")]
        public async Task<ActionResult<TourProblemDto>> SetDeadline(long id, [FromBody] SetDeadlineRequest request)
        {
            if (request == null) return BadRequest("Deadline request is required.");

            var deadlineUtc = request.DeadlineAt.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(request.DeadlineAt, DateTimeKind.Utc)
                : request.DeadlineAt.ToUniversalTime();

            try
            {
                var result = await _tourProblemService.SetDeadline(id, deadlineUtc);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }

    public class SetDeadlineRequest
    {
        public DateTime DeadlineAt { get; set; }
    }
}
