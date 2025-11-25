using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author.Authoring;

[ApiController]
[Route("api/quizzes")]
public class QuizController : ControllerBase
{
    private readonly IQuizService _quizService;

    public QuizController(IQuizService quizService)
    {
        _quizService = quizService;
    }

    [HttpGet("owned")]
    [Authorize(Policy = "authorPolicy")]
    public ActionResult<List<QuizDto>> GetOwned()
    {
        return Ok(_quizService.GetOwned(User.PersonId()));
    }

    [HttpPost]
    [Authorize(Policy = "authorPolicy")]
    public ActionResult<QuizDto> Create([FromBody] QuizDto quiz)
    {
        return Ok(_quizService.Create(quiz, User.PersonId()));
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "authorPolicy")]
    public ActionResult<QuizDto> Update(long id, [FromBody] QuizDto quiz)
    {
        quiz.Id = id;
        return Ok(_quizService.Update(quiz, User.PersonId()));
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "authorPolicy")]
    public ActionResult Delete(long id)
    {
        _quizService.Delete(id, User.PersonId());
        return Ok();
    }

    [HttpGet]
    [AllowAnonymous]
    public ActionResult<List<QuizDto>> GetAll()
    {
        var quizzes = _quizService.GetAllForTourists();
        foreach (var quiz in quizzes)
        {
            foreach (var question in quiz.Questions ?? new List<QuizQuestionDto>())
            {
                foreach (var option in question.Options ?? new List<QuizAnswerOptionDto>())
                {
                    option.IsCorrect = false;
                }
            }
        }

        return Ok(quizzes);
    }

    [HttpPost("{quizId:long}/submit")]
    [Authorize(Policy = "touristPolicy")]
    public ActionResult<QuizEvaluationResultDto> Submit(long quizId, [FromBody] SubmitQuizAnswersDto submission)
    {
        submission.QuizId = quizId;
        return Ok(_quizService.SubmitAnswers(submission, User.PersonId()));
    }
}