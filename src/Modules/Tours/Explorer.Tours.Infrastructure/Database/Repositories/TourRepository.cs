using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourRepository: ITourRepository

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
        return DbContext.Tours
            .Include(t => t.Equipment)
            .Include(t => t.KeyPoints)
            .ToList();
    }

    public PagedResult<Tour> GetPaged(int page, int pageSize)
    {
        var query = DbContext.Tours
            .Include(t => t.Equipment)
            .Include(t => t.KeyPoints)
            .OrderBy(t => t.Id);

        var totalCount = query.Count();

        var items = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<Tour>(items, totalCount);
    }

    public Tour Get(long id)
    {
        var entity = DbContext.Tours
        .Include(t => t.Equipment)
        .Include(t => t.KeyPoints)
        .FirstOrDefault(t => t.Id == id);

        if (entity == null)
            throw new NotFoundException("Not found: " + id);

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
        var existingTour = Get(tour.Id);
        if (existingTour == null)
            throw new NotFoundException("Not found: " + tour.Id);

        DbContext.Entry(existingTour).CurrentValues.SetValues(tour);
        DbContext.Entry(existingTour).Property(t => t.Duration).IsModified = true;

        try
        {
            DbContext.SaveChanges();
        }
        catch (DbUpdateException e)
        {
            throw new NotFoundException(e.Message);
        }
        return existingTour;
    }

    public void Delete(long id)
    {
        var entity = Get(id);
        _dbSet.Remove(entity);
        DbContext.SaveChanges();
    }
}
