using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain.Quiz;

public class Quiz : Entity
{
    public long AuthorId { get; private set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; set; }
    public ICollection<QuizQuestion> Questions { get; set; }

    public Quiz(long authorId, string title, string? description, DateTime createdAt, DateTime? updatedAt,
        ICollection<QuizQuestion> questions)
    {
        if (authorId <= 0) throw new ArgumentException("Invalid AuthorId.");
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Invalid Title.");
        if (createdAt == default) throw new ArgumentException("Invalid CreatedAt.");

        AuthorId = authorId;
        Title = title;
        Description = description;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        Questions = questions ?? new List<QuizQuestion>();
    }

    private Quiz()
    {
        Title = string.Empty;
        Questions = new List<QuizQuestion>();
    }

    public void Update(string title, string? description, DateTime updatedAt, ICollection<QuizQuestion> questions)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Invalid Title.");
        if (updatedAt == default) throw new ArgumentException("Invalid UpdatedAt.");

        Title = title;
        Description = description;
        UpdatedAt = updatedAt;
        Questions = questions ?? new List<QuizQuestion>();
    }
}
