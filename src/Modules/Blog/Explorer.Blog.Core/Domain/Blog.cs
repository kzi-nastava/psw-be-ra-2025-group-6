using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Blog.Core.Domain;

public class Blog : Entity
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public List<string> Images { get; private set; }

    public Blog(string title, string description, List<string> images)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.");

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required.");

        Title = title;
        Description = description;
        Images = images ?? new List<string>();
        CreatedAt = DateTime.UtcNow;
    }
}