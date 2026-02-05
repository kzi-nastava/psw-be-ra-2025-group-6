using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/quiz-encounters")]
    [ApiController]
    public class TouristQuizEncounterController : ControllerBase
    {
        private readonly IQuizEncounterService _quizEncounterService;

        public TouristQuizEncounterController(IQuizEncounterService quizEncounterService)
        {
            _quizEncounterService = quizEncounterService;
        }

        [HttpGet("by-challenge/{challengeId:long}")]
        public ActionResult<QuizEncounterDto> GetQuizEncounterByChallengeId(long challengeId)
        {
            var result = _quizEncounterService.GetForTourist(challengeId);
            return Ok(result);
        }

        [HttpPost("submit")]
        public ActionResult<QuizResultDto> SubmitQuizAnswers([FromBody] QuizSubmissionDto dto)
        {
            try
            {
                var userId = User.PersonId();
                var result = _quizEncounterService.SubmitQuiz(dto, userId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while submitting the quiz.", error = ex.Message });
            }
        }

        [HttpGet("completed/{challengeId:long}")]
        public ActionResult<bool> HasCompletedQuiz(long challengeId)
        {
            var userId = User.PersonId();
            var result = _quizEncounterService.HasTouristCompletedQuiz(userId, challengeId);
            return Ok(result);
        }
    }
}
