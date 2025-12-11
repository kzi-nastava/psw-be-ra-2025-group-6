using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class TourReview : Entity
{
    public long UserId { get; private set; }
    public long TourId { get; private set; }
    public int Rating { get; private set; }
    public string? Comment { get; private set; }
    public int CompletedPercent { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public TourReview(long userId, long tourId, int rating, string? comment, int completedPercent)
    {
        UserId = userId;
        TourId = tourId;
        Rating = rating;
        Comment = comment;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null;
        CompletedPercent = completedPercent;
        Validate();
    }

    private void Validate()
    {
        if (UserId == 0) throw new ArgumentException("Invalid PersonId");
        if (Rating < 0 || Rating > 5) throw new ArgumentException("Rating must be between 1 and 5.");
        if (CompletedPercent < 0 || CompletedPercent > 100) throw new ArgumentException("CompletedPercent must be between 0 and 100.");
    }
}
