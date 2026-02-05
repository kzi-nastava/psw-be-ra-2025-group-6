using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Encounters
{
    [ApiController]
    [Route("api/encounters/hidden-location")]
    public class HiddenLocationController : ControllerBase
    {
        private readonly IHiddenLocationService _service;

        public HiddenLocationController(IHiddenLocationService service)
        {
            _service = service;
        }

        [HttpPost("start")]
        [Authorize(Policy = "touristPolicy")]
        public ActionResult<HiddenLocationAttemptDto> StartAttempt([FromBody] StartHiddenLocationDto dto)
        {
            try
            {
                var userId = User.PersonId();
                var result = _service.StartAttempt(dto, userId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("progress")]
        [Authorize(Policy = "touristPolicy")]
        public ActionResult<HiddenLocationProgressDto> UpdateProgress([FromBody] UpdateHiddenLocationProgressDto dto)
        {
            try
            {
                var userId = User.PersonId();
                var result = _service.UpdateProgress(dto, userId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpGet("active/{challengeId:long}")]
        [Authorize(Policy = "touristPolicy")]
        public ActionResult<HiddenLocationAttemptDto> GetActiveAttempt(long challengeId)
        {
            var userId = User.PersonId();
            var result = _service.GetActiveAttempt(userId, challengeId);
            
            if (result == null)
                return NotFound(new { message = "No active attempt found." });
            
            return Ok(result);
        }

        [HttpGet("attempts/{challengeId:long}")]
        [Authorize(Policy = "touristPolicy")]
        public ActionResult<List<HiddenLocationAttemptDto>> GetUserAttempts(long challengeId)
        {
            var userId = User.PersonId();
            var result = _service.GetUserAttempts(userId, challengeId);
            return Ok(result);
        }

        [HttpGet("check-activation/{challengeId:long}")]
        [Authorize(Policy = "touristPolicy")]
        public ActionResult<ActivationCheckDto> CheckActivation(long challengeId, [FromQuery] double latitude, [FromQuery] double longitude)
        {
            try
            {
                var result = _service.CheckActivation(challengeId, latitude, longitude);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
