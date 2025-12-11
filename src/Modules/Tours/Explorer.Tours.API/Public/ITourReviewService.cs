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
    }
}
