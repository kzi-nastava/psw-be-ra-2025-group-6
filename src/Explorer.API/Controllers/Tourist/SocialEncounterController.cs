using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/social-encounters")]
    [ApiController]
    public class SocialEncounterController : ControllerBase
    {
        private readonly ISocialEncounterService _socialEncounterService;

        public SocialEncounterController(ISocialEncounterService socialEncounterService)
        {
            _socialEncounterService = socialEncounterService;
        }

        [HttpPost("{challengeId}/activate")]
        public ActionResult<ActivateSocialEncounterResponseDto> ActivateSocialEncounter(
            long challengeId,
            [FromBody] ActivateSocialEncounterRequestDto request)
        {
            var userId = long.Parse(User.Claims.First(c => c.Type == "id").Value);
            var result = _socialEncounterService.ActivateSocialEncounter(challengeId, userId, request);
            return Ok(result);
        }

        [HttpPost("{challengeId}/heartbeat")]
        public ActionResult<SocialEncounterHeartbeatResponseDto> SendHeartbeat(
            long challengeId,
            [FromBody] SocialEncounterHeartbeatRequestDto request)
        {
            var userId = long.Parse(User.Claims.First(c => c.Type == "id").Value);
            var result = _socialEncounterService.SendHeartbeat(challengeId, userId, request);
            return Ok(result);
        }

        [HttpPost("{challengeId}/deactivate")]
        public ActionResult<DeactivateSocialEncounterResponseDto> DeactivateSocialEncounter(long challengeId)
        {
            var userId = long.Parse(User.Claims.First(c => c.Type == "id").Value);
            var result = _socialEncounterService.DeactivateSocialEncounter(challengeId, userId);
            return Ok(result);
        }

        [HttpGet("challenge/{challengeId}")]
        public ActionResult<SocialEncounterDto> GetByChallengeId(long challengeId)
        {
            var result = _socialEncounterService.GetByChallengeId(challengeId);
            if (result == null)
                return NotFound();
            return Ok(result);
        }


    }
}