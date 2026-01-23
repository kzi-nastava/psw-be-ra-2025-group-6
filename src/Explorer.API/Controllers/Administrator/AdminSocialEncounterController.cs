using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator
{
    [Authorize(Policy = "administratorPolicy")]
    [Route("api/administrator/social-encounters")]
    [ApiController]
    public class AdminSocialEncounterController : ControllerBase
    {
        private readonly ISocialEncounterService _socialEncounterService;

        public AdminSocialEncounterController(ISocialEncounterService socialEncounterService)
        {
            _socialEncounterService = socialEncounterService;
        }

        [HttpPost]
        public ActionResult<SocialEncounterDto> Create([FromBody] SocialEncounterDto dto)
        {
            var result = _socialEncounterService.CreateSocialEncounter(dto);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public ActionResult<SocialEncounterDto> Update(long id, [FromBody] SocialEncounterDto dto)
        {
            var result = _socialEncounterService.UpdateSocialEncounter(id, dto);
            return Ok(result);
        }

        [HttpGet]
        public ActionResult<List<SocialEncounterDto>> GetAll()
        {
            var result = _socialEncounterService.GetAll();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public ActionResult<SocialEncounterDto> Get(long id)
        {
            var result = _socialEncounterService.GetSocialEncounter(id);
            if (result == null)
                return NotFound();
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

        [HttpDelete("{id}")]
        public ActionResult Delete(long id)
        {
            _socialEncounterService.DeleteSocialEncounter(id);
            return NoContent();
        }
    }
}