using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourRepository : ITourRepository
{
    protected readonly ToursContext DbContext;
    private readonly DbSet<Tour> _dbSet;

    public TourRepository(ToursContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<Tour>();
    }

    public List<Tour> GetAll()
    {
        return _dbSet.Include(t => t.TourReviews).ToList();
    }

    public PagedResult<Tour> GetPaged(int page, int pageSize)
    {
        var totalCount = _dbSet.Count();
        var items = _dbSet
            .Include(t => t.TourReviews)
            .OrderBy(e => e.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<Tour>(items, totalCount);


    }

    public Tour Get(long id)
    {
        var entity = _dbSet.Include(t => t.TourReviews).FirstOrDefault(t => t.Id == id);
        if (entity == null) throw new NotFoundException("Not found: " + id);
        return entity;
    }

    public Tour Create(Tour tour)
    {
        _dbSet.Add(tour);
        DbContext.SaveChanges();
        return tour;
    }

    public Tour Update(Tour tour)
    {
        try
        {
            DbContext.Update(tour);
            DbContext.SaveChanges();
        }
        catch (DbUpdateException e)
        {
            throw new NotFoundException(e.Message);
        }
        return tour;
    }

    public Tour? GetByReviewId(long reviewId)
    {
        return _dbSet.Include(t => t.TourReviews).FirstOrDefault(t => t.TourReviews.Any(r => r.Id == reviewId));
    }

    public List<Tour> GetByReviewUserId(long userId)
    {
        return _dbSet.Include(t => t.TourReviews).Where(t => t.TourReviews.Any(r => r.UserId == userId)).ToList();
    }

    public void Delete(long id)
    {
        var entity = Get(id);
        _dbSet.Remove(entity);
        DbContext.SaveChanges();
    }
}
