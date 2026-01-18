using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Blog.Core.Domain;

public class Comment : Entity
{
    public long BlogId { get; private set; }
    public long UserId { get; private set; }
    public string AuthorName { get; private set; }
    public string AuthorProfilePicture { get; private set; }
    public string Text { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastUpdatedAt { get; private set; }
    public bool IsHidden { get; private set; }

    private Comment() { }
    public Comment(long blogId, long userId, string authorName, string authorProfilePicture, string text)
    {
        if (userId == 0)
            throw new ArgumentException("Invalid user.");
        
        if(string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text required.");

        if (string.IsNullOrWhiteSpace(authorName))
            throw new ArgumentException("Author name required.");

        BlogId = blogId;
        UserId = userId;
        AuthorName = authorName;
        AuthorProfilePicture = authorProfilePicture;
        Text = text;
        CreatedAt = DateTime.UtcNow;
        IsHidden = false;
    }

    public Comment(long blogId, long userId, string authorName, string authorProfilePicture, string text, DateTime createdAt, DateTime? lastUpdatedAt)
    {
        BlogId = blogId;
        UserId = userId;
        AuthorName = authorName;
        AuthorProfilePicture = authorProfilePicture;
        Text = text;
        CreatedAt = createdAt;
        LastUpdatedAt = lastUpdatedAt;
        IsHidden = false;
    }

    public void Edit(string newText)
    {
        if (string.IsNullOrWhiteSpace(newText))
            throw new ArgumentException("Text required");

        Text = newText.Trim();
        LastUpdatedAt = DateTime.UtcNow;
    }

    public void Hide(long adminId)
    {
        IsHidden = true;
    }
}
