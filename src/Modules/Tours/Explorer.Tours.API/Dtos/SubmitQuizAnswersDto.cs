namespace Explorer.Tours.API.Dtos;

public class SubmitQuizAnswersDto
{
    public long QuizId { get; set; }
    public List<QuestionAnswerDto> Answers { get; set; } = new();
}

public class QuestionAnswerDto
{
    public long QuestionId { get; set; }
    public List<long> SelectedOptionIds { get; set; } = new();
}