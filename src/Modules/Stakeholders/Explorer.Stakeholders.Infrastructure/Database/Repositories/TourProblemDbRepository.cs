using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class TourProblemDbRepository : ITourProblemRepository
{
    protected readonly StakeholdersContext DbContext;
    private readonly DbSet<TourProblem> _dbSet;

    public TourProblemDbRepository(StakeholdersContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<TourProblem>();
    }

    public async Task<List<TourProblem>> GetByTourist(long touristId)
    {
        return await _dbSet
            .Where(p => p.TouristId == touristId)
            .ToListAsync();
    }

    public async Task<TourProblem?> GetById(long id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity == null) throw new NotFoundException("TourProblem not found: " + id);
        return entity;
    }

    public async Task<TourProblem> Create(TourProblem problem)
    {
        _dbSet.Add(problem);
        await DbContext.SaveChangesAsync();
        return problem;
    }

    public async Task<TourProblem> Update(TourProblem problem)
    {
        try
        {
            DbContext.Update(problem);
            await DbContext.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            throw new NotFoundException(e.Message);
        }
        return problem;
    }

    public async Task Delete(long id)
    {
        var entity = await GetById(id);
        _dbSet.Remove(entity);
        await DbContext.SaveChangesAsync();
    }
}