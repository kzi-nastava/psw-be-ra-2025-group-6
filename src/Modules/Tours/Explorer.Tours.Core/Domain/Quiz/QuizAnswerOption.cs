using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain.Quiz;

public class QuizAnswerOption : Entity
{
    public long QuestionId { get; set; }
    public string Text { get; set; }
    public bool IsCorrect { get; set; }
    public string Feedback { get; set; }

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
