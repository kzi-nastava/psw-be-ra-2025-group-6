using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Blog.Core.Domain;

public class BlogPost : Entity
{
    public long UserId { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public List<string> Images { get; private set; }
    public BlogStatus Status { get; set; }
    public DateTime? LastModifiedAt { get; private set; }
    public BlogQualityStatus QualityStatus { get; private set; } = BlogQualityStatus.None;


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

    public int GetUpvotes(IBlogVoteRepository voteRepo)
        => voteRepo.CountUpvotes(this.Id);

    public int GetDownvotes(IBlogVoteRepository voteRepo)
        => voteRepo.CountDownvotes(this.Id);

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
}