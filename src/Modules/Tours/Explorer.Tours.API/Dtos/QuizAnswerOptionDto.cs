namespace Explorer.Tours.API.Dtos;

public class QuizAnswerOptionDto
{
    public long Id { get; set; }
    public long QuestionId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Feedback { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}