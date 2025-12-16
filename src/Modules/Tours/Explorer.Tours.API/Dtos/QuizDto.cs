namespace Explorer.Tours.API.Dtos;

public class QuizDto
{
    public long Id { get; set; }
    public long AuthorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<QuizQuestionDto> Questions { get; set; } = new();
}