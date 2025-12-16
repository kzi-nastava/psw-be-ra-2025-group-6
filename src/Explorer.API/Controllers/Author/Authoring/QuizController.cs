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
        if (!TryGetPersonId(out var personId)) return Unauthorized();
        return Ok(_quizService.GetOwned(personId));
    }

    [HttpPost]
    [Authorize(Policy = "authorPolicy")]
    public ActionResult<QuizDto> Create([FromBody] QuizDto quiz)
    {
        if (!TryGetPersonId(out var personId)) return Unauthorized();
        return Ok(_quizService.Create(quiz, personId));
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "authorPolicy")]
    public ActionResult<QuizDto> Update(long id, [FromBody] QuizDto quiz)
    {
        if (!TryGetPersonId(out var personId)) return Unauthorized();
        quiz.Id = id;
        return Ok(_quizService.Update(quiz, personId));
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "authorPolicy")]
    public ActionResult Delete(long id)
    {
        if (!TryGetPersonId(out var personId)) return Unauthorized();
        _quizService.Delete(id, personId);
        return Ok();
    }

    [HttpDelete("{quizId:long}/questions/{questionId:long}")]
    [Authorize(Policy = "authorPolicy")]
    public ActionResult DeleteQuestion(long quizId, long questionId)
    {
        if (!TryGetPersonId(out var personId)) return Unauthorized();
        _quizService.DeleteQuestion(quizId, questionId, personId);
        return Ok();
    }

    [HttpDelete("{quizId:long}/questions/{questionId:long}/options/{optionId:long}")]
    [Authorize(Policy = "authorPolicy")]
    public ActionResult DeleteOption(long quizId, long questionId, long optionId)
    {
        if (!TryGetPersonId(out var personId)) return Unauthorized();
        _quizService.DeleteOption(quizId, questionId, optionId, personId);
        return Ok();
    }

    [HttpGet]
    [Authorize(Policy = "touristPolicy")]
    public ActionResult<List<QuizDto>> GetAll()
    {
        var quizzes = _quizService.GetAllForTourists();
        RemoveCorrectAnswersForTourists(quizzes);

        return Ok(quizzes);
    }

    [HttpPost("{quizId:long}/submit")]
    [Authorize(Policy = "touristPolicy")]
    public ActionResult<QuizEvaluationResultDto> Submit(long quizId, [FromBody] SubmitQuizAnswersDto submission)
    {
        if (!TryGetPersonId(out var personId)) return Unauthorized();
        submission.QuizId = quizId;
        return Ok(_quizService.SubmitAnswers(submission, personId));
    }

    // Alias for submitting quiz answers using /solve route expected by some clients.
    [HttpPost("{quizId:long}/solve")]
    [Authorize(Policy = "touristPolicy")]
    public ActionResult<QuizEvaluationResultDto> Solve(long quizId, [FromBody] SubmitQuizAnswersDto submission)
    {
        if (!TryGetPersonId(out var personId)) return Unauthorized();
        submission.QuizId = quizId;
        return Ok(_quizService.SubmitAnswers(submission, personId));
    }

    private static void RemoveCorrectAnswersForTourists(IEnumerable<QuizDto> quizzes)
    {
        // Prevent leaking correct answers when quizzes are listed to tourists.
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
    }

    private bool TryGetPersonId(out long personId)
    {
        personId = User.PersonId();
        return personId > 0;
    }
}
