using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain.Quiz;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Tours.Core.UseCases.Authoring;

public class QuizEvaluator
{
    public QuizEvaluationResultDto Evaluate(Quiz quiz, SubmitQuizAnswersDto submission)
    {
        var submittedAnswers = submission.Answers ?? new List<QuestionAnswerDto>();
        var questionResults = quiz.Questions
            .Select(question => EvaluateQuestion(question, submittedAnswers))
            .ToList();

        return new QuizEvaluationResultDto
        {
            QuizId = quiz.Id,
            Questions = questionResults
        };
    }

    private static QuestionEvaluationResultDto EvaluateQuestion(QuizQuestion question, List<QuestionAnswerDto> submittedAnswers)
    {
        var submitted = submittedAnswers.FirstOrDefault(a => a.QuestionId == question.Id);
        var selected = submitted?.SelectedOptionIds ?? new List<long>();
        var correctOptions = question.Options.Where(o => o.IsCorrect).Select(o => o.Id).ToList();
        var isCompletelyCorrect = selected.Count == correctOptions.Count &&
                                  !correctOptions.Except(selected).Any() &&
                                  !selected.Except(correctOptions).Any();

        var optionEvaluations = question.Options.Select(option => new OptionEvaluationDto
        {
            OptionId = option.Id,
            Text = option.Text,
            IsCorrect = option.IsCorrect,
            IsSelected = selected.Contains(option.Id),
            Feedback = option.Feedback
        }).ToList();

        return new QuestionEvaluationResultDto
        {
            QuestionId = question.Id,
            IsCompletelyCorrect = isCompletelyCorrect,
            Options = optionEvaluations
        };
    }
}
