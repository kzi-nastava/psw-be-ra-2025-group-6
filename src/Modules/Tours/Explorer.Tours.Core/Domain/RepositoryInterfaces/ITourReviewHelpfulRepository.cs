namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourReviewHelpfulRepository
{
    bool Exists(long reviewId, long userId);
    void Add(Explorer.Tours.Core.Domain.TourReviewHelpfulVote vote);
    void RemoveByReviewAndUser(long reviewId, long userId);
    int CountByReview(long reviewId);
}