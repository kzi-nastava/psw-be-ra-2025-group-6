using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public
{
    public interface ITourReviewService
    {
        TourReviewDto Create(TourReviewDto club);
        TourReviewDto Update(TourReviewDto club);
        void Delete(long id);
        TourReviewDto Get(long id);
        List<TourReviewDto> GetByUser(long userId);
        List<TourReviewDto> GetByTour(long tourId);

        (int helpfulCount, bool isHelpful) ToggleHelpful(long reviewId, long userId);

        // NEW: remove helpful vote (returns updated count and isHelpful=false)
        (int helpfulCount, bool isHelpful) RemoveHelpful(long reviewId, long userId);
    }
}
