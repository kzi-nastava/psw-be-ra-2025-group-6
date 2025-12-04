namespace Explorer.Tours.API.Dtos;

public class QuizEvaluationResultDto
{
    public long QuizId { get; set; }
    public List<QuestionEvaluationResultDto> Questions { get; set; } = new();
}

public class QuestionEvaluationResultDto
{
    public long QuestionId { get; set; }
    public bool IsCompletelyCorrect { get; set; }
    public List<OptionEvaluationDto> Options { get; set; } = new();
}

public class OptionEvaluationDto
{
    public long OptionId { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public bool IsSelected { get; set; }
    public string Feedback { get; set; } = string.Empty;
}