using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IReviewAppRepository
    {
        ReviewApp Get(long id);
        List<ReviewApp> GetByUser(long userId);
        List<ReviewApp> GetAll();
        void Add(ReviewApp review);
        void Update(ReviewApp review);
        void Delete(ReviewApp review);
        PagedResult<ReviewApp> GetPaged(int page, int pageSize);
    }
}
