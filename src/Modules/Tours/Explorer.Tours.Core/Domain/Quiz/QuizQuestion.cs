using Explorer.BuildingBlocks.Core.Domain;
using System.Linq;

namespace Explorer.Tours.Core.Domain.Quiz;

public class QuizQuestion : Entity
{
    public long QuizId { get; private set; }
    public string Text { get; private set; } = string.Empty;
    public bool AllowsMultipleAnswers { get; private set; }
    public ICollection<QuizAnswerOption> Options { get; private set; }

    public QuizQuestion(long quizId, string text, bool allowsMultipleAnswers, ICollection<QuizAnswerOption> options)
    {
       
        if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("Invalid Text.");
        ValidateCorrectAnswers(allowsMultipleAnswers, options);

        QuizId = quizId;
        Text = text;
        AllowsMultipleAnswers = allowsMultipleAnswers;
        Options = options ?? new List<QuizAnswerOption>();
    }

    private QuizQuestion()
    {
        Options = new List<QuizAnswerOption>();
    }

    private static void ValidateCorrectAnswers(bool allowsMultipleAnswers, ICollection<QuizAnswerOption>? options)
    {
        if (options == null || options.Count == 0) throw new ArgumentException("Question must contain answer options.");
        var correctCount = options.Count(o => o.IsCorrect);
        if (correctCount == 0) throw new ArgumentException("Question must contain at least one correct answer.");
        if (!allowsMultipleAnswers && correctCount != 1) throw new ArgumentException("Single-choice question must have exactly one correct answer.");
    }
}