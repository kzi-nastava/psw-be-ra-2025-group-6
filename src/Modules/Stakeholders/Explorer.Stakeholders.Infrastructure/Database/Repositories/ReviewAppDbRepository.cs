using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class ReviewAppDbRepository : IReviewAppRepository
    {
        private readonly StakeholdersContext _dbContext;

        public ReviewAppDbRepository(StakeholdersContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ReviewApp Get(long id)
        {
            return _dbContext.ReviewApps.FirstOrDefault(r => r.Id == id);
        }
        public void Delete(ReviewApp review)
        {
            _dbContext.ReviewApps.Remove(review);
            _dbContext.SaveChanges();
        }

        public List<ReviewApp> GetByUser(long userId)
        {
            return _dbContext.ReviewApps
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
        }

        public List<ReviewApp> GetAll()
        {
            return _dbContext.ReviewApps
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
        }

        public void Add(ReviewApp review)
        {
            _dbContext.ReviewApps.Add(review);
            _dbContext.SaveChanges();
        }

        public void Update(ReviewApp review)
        {
            _dbContext.ReviewApps.Update(review);
            _dbContext.SaveChanges();
        }

        public PagedResult<ReviewApp> GetPaged(int page, int pageSize)
        {
            var query = _dbContext.ReviewApps.AsQueryable();

            var totalCount = query.Count();

            var items = query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<ReviewApp>(items, totalCount);
        }
    }
}
