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

    public BlogStatus Status { get; set; }
    public DateTime? LastModifiedAt { get; private set; }
    public BlogQualityStatus QualityStatus { get; private set; } = BlogQualityStatus.None;

    private readonly List<BlogVote> _votes = new();
    public IReadOnlyCollection<BlogVote> Votes => _votes.AsReadOnly();

    private BlogPost() { }

    public BlogPost(long userId, string title, string description, List<string> images, BlogStatus status)
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
        Status = status;

    }
    public void AddImages(List<string> imagePaths)
    {
        if (imagePaths == null || !imagePaths.Any())
            return;

        Images = Images.Concat(imagePaths).ToList();
    }

    public void AddComment(long userId, string authorName, string authorProfilePicture, string text)
    {
        if (Comments == null)
        {
            Comments = new List<Comment>();
        }

        var comment = new Comment(userId, authorName, authorProfilePicture, text);
        Comments.Add(comment);

        RecalculateQualityStatus();
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
    public void UpdateDescription(string newDescription)
    {
        if (string.IsNullOrWhiteSpace(newDescription))
            throw new ArgumentException("Description cannot be empty.");

        Description = newDescription;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void UpdateTitle(string newTitle)
    {
        if (string.IsNullOrWhiteSpace(newTitle))
            throw new Exception("Title cannot be empty");

        Title = newTitle;
    }

    public void AddOrUpdateVote(long userId, VoteType type)
    {
        var existing = _votes.FirstOrDefault(v => v.UserId == userId);
        if (existing != null)
        {
            existing.UpdateVote(type); 
        }
        else
        {
            _votes.Add(new BlogVote(userId, type));
        }
    }

    public void RemoveVote(long userId)
    {
        var existing = _votes.FirstOrDefault(v => v.UserId == userId);
        if (existing != null)
        {
            _votes.Remove(existing);
        }
    }

    public int CountUpvotes()
        => _votes.Count(v => v.Type == VoteType.Upvote);

    public int CountDownvotes()
        => _votes.Count(v => v.Type == VoteType.Downvote);

    public void UpdateQualityStatus(int score, int commentCount)
    {
        if(score < -10)
        {
            QualityStatus = BlogQualityStatus.Closed;
            return;
        }
        if(score > 500 && commentCount > 30)
        {
            QualityStatus = BlogQualityStatus.Famous;
            return;
        }

        if (score > 100 || commentCount > 10)
        {
            QualityStatus = BlogQualityStatus.Active;
            return;
        }

        QualityStatus = BlogQualityStatus.None;
    }

    public void RecalculateQualityStatus()
    {
        int score = CountUpvotes() - CountDownvotes();
        int commentCount = Comments.Count;

        UpdateQualityStatus(score, commentCount);
    }
}