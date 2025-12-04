using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

public class TourReview : Entity
{
    public long UserId { get; private set; }
    public long TourId { get; private set; }
    public int Rating { get; private set; }
    public string? Comment { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }


    public TourReview(long userId, long tourId, int rating, string? comment)
    {
        UserId = userId;
        TourId = tourId;
        Rating = rating;
        Comment = comment;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null;
        Validate();
    }

    private void Validate()
    {
        if (UserId == 0) throw new ArgumentException("Invalid PersonId");
        if (Rating < 0 || Rating > 5) throw new ArgumentException("Rating must be between 1 and 5.");
    }
}
