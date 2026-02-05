using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author.Authoring
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/author/quiz-encounters")]
    [ApiController]
    public class QuizEncounterController : ControllerBase
    {
        private readonly IQuizEncounterService _quizEncounterService;

        public QuizEncounterController(IQuizEncounterService quizEncounterService)
        {
            _quizEncounterService = quizEncounterService;
        }

        [HttpPost]
        public ActionResult<QuizEncounterDto> CreateQuizEncounter([FromBody] CreateQuizEncounterDto dto)
        {
            var authorId = User.PersonId();
            var result = _quizEncounterService.CreateForKeyPoint(dto, authorId);
            return Ok(result);
        }

        [HttpPut("{id:long}")]
        public ActionResult<QuizEncounterDto> UpdateQuizEncounter(long id, [FromBody] UpdateQuizEncounterDto dto)
        {
            dto.Id = id;
            var authorId = User.PersonId();
            var result = _quizEncounterService.Update(dto, authorId);
            return Ok(result);
        }

        [HttpDelete("{id:long}")]
        public ActionResult DeleteQuizEncounter(long id)
        {
            var authorId = User.PersonId();
            _quizEncounterService.Delete(id, authorId);
            return NoContent();
        }

        [HttpGet("{id:long}")]
        public ActionResult<QuizEncounterDto> GetQuizEncounterById(long id)
        {
            var result = _quizEncounterService.GetById(id);
            return Ok(result);
        }

        [HttpGet("by-challenge/{challengeId:long}")]
        public ActionResult<QuizEncounterDto> GetQuizEncounterByChallengeId(long challengeId)
        {
            var result = _quizEncounterService.GetByChallengeId(challengeId);
            return Ok(result);
        }

        [HttpGet("my-quizzes")]
        public ActionResult<List<QuizEncounterDto>> GetMyQuizEncounters()
        {
            var authorId = User.PersonId();
            var result = _quizEncounterService.GetByAuthor(authorId);
            return Ok(result);
        }
    }
}
