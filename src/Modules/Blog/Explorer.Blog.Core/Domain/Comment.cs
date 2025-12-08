using Explorer.BuildingBlocks.Core.Domain;
using System.Text.Json.Serialization;

namespace Explorer.Blog.Core.Domain;

public class Comment : ValueObject
{
    public long UserId { get; private set; }
    public string AuthorName { get; private set; }
    public string Text { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastUpdatedAt { get; private set; }

    private Comment() { }
    public Comment(long userId, string authorName, string text)
    {
        if (userId == 0)
            throw new ArgumentException("Invalid user.");
        
        if(string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text required.");

        if (string.IsNullOrWhiteSpace(authorName))
            throw new ArgumentException("Author name required.");

        UserId = userId;
        AuthorName = authorName;
        Text = text;
        CreatedAt = DateTime.UtcNow;
    }

    [JsonConstructor]
    public Comment(long userId, string authorName, string text, DateTime createdAt, DateTime? lastUpdatedAt)
    {
        UserId = userId;
        AuthorName = authorName;
        Text = text;
        CreatedAt = createdAt;
        LastUpdatedAt = lastUpdatedAt;
    }

    public void Edit(string newText)
    {
        if (string.IsNullOrWhiteSpace(newText))
            throw new ArgumentException("Text required");

        Text = newText.Trim();
        LastUpdatedAt = DateTime.UtcNow;
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return UserId;
        yield return AuthorName;
        yield return Text;
        yield return CreatedAt;
        yield return LastUpdatedAt;
    }
}
