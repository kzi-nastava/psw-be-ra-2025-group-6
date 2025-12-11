using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourReviewDbRepository : ITourReviewRepository
{
    private readonly ToursContext _dbContext;

    public TourReviewDbRepository(ToursContext dbContext)
    {
        _dbContext = dbContext;
    }

    public TourReview Get(long id)
    {
        return _dbContext.TourReviews.FirstOrDefault(r => r.Id == id);
    }

    public List<TourReview> GetByUser(long userId)
    {
        return _dbContext.TourReviews
                .Where(r => r.UserId == userId)
                .ToList();
    }
    public List<TourReview> GetByTour(long tourId)
    {
        return _dbContext.TourReviews
                .Where(r => r.TourId == tourId)
                .ToList();
    }
    public void Delete(TourReview r)
    {
        _dbContext.TourReviews.Remove(r);
        _dbContext.SaveChanges();
    }



    public List<TourReview> GetAll()
    {
        return _dbContext.TourReviews
            .OrderByDescending(r => r.CreatedAt)
            .ToList();
    }

    public void Create(TourReview review)
    {
        _dbContext.TourReviews.Add(review);
        _dbContext.SaveChanges();
    }

    public void Update(TourReview review)
    {
        _dbContext.TourReviews.Update(review);
        _dbContext.SaveChanges();
    }

    public PagedResult<TourReview> GetPaged(int page, int pageSize)
    {
        var query = _dbContext.TourReviews.AsQueryable();

        var totalCount = query.Count();

        var items = query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<TourReview>(items, totalCount);
    }
}

