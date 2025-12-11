using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface ITourReviewRepository
    {
        PagedResult<TourReview> GetPaged(int page, int pageSize);
        TourReview Get(long id);
        List<TourReview> GetByUser(long userId);
        List<TourReview> GetByTour(long tourId);
        void Create(TourReview tourReview);
        void Update(TourReview tourReview);
        void Delete(TourReview r);
    }


}
