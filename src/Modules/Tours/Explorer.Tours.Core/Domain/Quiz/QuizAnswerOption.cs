using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain.Quiz;

public class QuizAnswerOption : Entity
{
    public long QuestionId { get; private set; }
    public string Text { get; private set; }
    public bool IsCorrect { get; private set; }
    public string Feedback { get; private set; }

    public QuizAnswerOption(long questionId, string text, bool isCorrect, string feedback)
    {
        
        if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("Invalid Text.");
        if (string.IsNullOrWhiteSpace(feedback)) throw new ArgumentException("Invalid Feedback.");

        QuestionId = questionId;
        Text = text;
        IsCorrect = isCorrect;
        Feedback = feedback;
    }

    private QuizAnswerOption()
    {
        Feedback = "";
        Text = "";
    }
}