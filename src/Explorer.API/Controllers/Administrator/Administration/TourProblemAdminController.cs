using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
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
                var adminPersonId = User.PersonId();
                var result = await _tourProblemService.SetDeadline(id, deadlineUtc, adminPersonId);
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

        [HttpPost("{id}/finalize")]
        public async Task<ActionResult<TourProblemDto>> Finalize(long id, [FromBody] FinalizeProblemRequest request)
        {
            if (request == null) return BadRequest("Finalize payload is required.");

            try
            {
                var adminPersonId = User.PersonId();
                var result = await _tourProblemService.FinalizeStatus(id, request.Status, adminPersonId);
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
            catch (Explorer.BuildingBlocks.Core.Exceptions.NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }

    public class SetDeadlineRequest
    {
        public DateTime DeadlineAt { get; set; }
    }

    public class FinalizeProblemRequest
    {
        public int Status { get; set; }
    }
}
