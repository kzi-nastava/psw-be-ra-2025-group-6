using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourRepository<TDbContext> : ITourRepository where TDbContext : DbContext
{
    protected readonly TDbContext DbContext;
    private readonly DbSet<Tour> _dbSet;

    public TourRepository(TDbContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<Tour>();
    }

    public List<Tour> GetAll()
    {
        return _dbSet.ToList();
    }

    public List<Tour> GetAllWithKeyPoints()
    {
        return _dbSet.Include(t => t.KeyPoints).ToList();
    }

    public List<Tour> GetPublishedWithKeyPoints()
    {
        return _dbSet.Include(t => t.KeyPoints)
            .Where(t => t.Status == TourStatus.CONFIRMED)
            .ToList();
    }

    public PagedResult<Tour> GetPaged(int page, int pageSize)
    {
        var totalCount = _dbSet.Count();
        var items = _dbSet
            .OrderBy(e => e.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<Tour>(items, totalCount);
    }

    public Tour Get(long id)
    {
        var entity = _dbSet.Find(id);
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

    public void Delete(long id)
    {
        var entity = Get(id);
        _dbSet.Remove(entity);
        DbContext.SaveChanges();
    }
}
