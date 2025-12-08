using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Blog.Core.Domain;

public class BlogPost : AggregateRoot
{
    public long UserId { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public List<string> Images { get; private set; }
    public List<Comment> Comments { get; private set; } = new();

    private BlogPost() { }

    public BlogPost(long userId, string title, string description, List<string> images)
    {
        if (userId == 0) throw new ArgumentException("Invalid UserId.");

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.");

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required.");

        UserId = userId;
        Title = title;
        Description = description;
        Images = images ?? new List<string>();
        CreatedAt = DateTime.UtcNow;
    }

    public void AddImages(List<string> imagePaths)
    {
        if (imagePaths == null || !imagePaths.Any())
            return;

        Images = Images.Concat(imagePaths).ToList();
    }

    public void AddComment(long userId, string authorName, string text)
    {
        if (Comments == null)
        {
            Comments = new List<Comment>();
        }

        var comment = new Comment(userId, authorName, text);
        Comments.Add(comment);
    }

    public void EditComment(int id, long userId, string text)
    {
        if (Comments == null || id < 0 || id >= Comments.Count)
            throw new InvalidOperationException("Comment does not exist.");

        var comment = Comments[id];

        if (comment.UserId != userId)
            throw new InvalidOperationException("Only authors can edit their comments.");

        if (DateTime.UtcNow - comment.CreatedAt > TimeSpan.FromMinutes(15))
            throw new InvalidOperationException("Edit time expired.");

        comment.Edit(text);
    }

    public void DeleteComment(int id, long userId)
    {
        if (Comments == null || id < 0 || id >= Comments.Count)
            throw new InvalidOperationException("Comment does not exist.");

        var comment = Comments[id];

        if (comment.UserId != userId)
            throw new InvalidOperationException("Only authors can delete their comments.");

        if (DateTime.UtcNow - comment.CreatedAt > TimeSpan.FromMinutes(15))
            throw new InvalidOperationException("Delete time expired.");

        Comments.RemoveAt(id);
    }
}