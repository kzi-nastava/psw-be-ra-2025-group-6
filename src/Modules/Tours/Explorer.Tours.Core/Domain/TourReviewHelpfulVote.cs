using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class TourReviewHelpfulVote : Entity
{
    public long ReviewId { get; init; }
    public long UserId { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    private TourReviewHelpfulVote() { }

    public TourReviewHelpfulVote(long reviewId, long userId)
    {
        if (reviewId == 0) throw new ArgumentException("Invalid ReviewId");
        if (userId == 0) throw new ArgumentException("Invalid UserId");

        ReviewId = reviewId;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
    }
}