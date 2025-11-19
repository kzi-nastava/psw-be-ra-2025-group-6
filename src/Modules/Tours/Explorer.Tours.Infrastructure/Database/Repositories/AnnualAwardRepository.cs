using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class AnnualAwardRepository<AnnualAward, TDbContext> : IAnnualAwardRepository<AnnualAward>
    where AnnualAward : Entity
    where TDbContext : DbContext
{
    protected readonly TDbContext DbContext;
    private readonly DbSet<AnnualAward> _dbSet;

    public AnnualAwardRepository(TDbContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<AnnualAward>();
    }
    public AnnualAward Create(AnnualAward award)
    {
        _dbSet.Add(award);
        DbContext.SaveChanges();
        return award;
    }

    public void Delete(long id)
    {
        var entity = Get(id);
        _dbSet.Remove(entity);
        DbContext.SaveChanges();
    }

    public AnnualAward Get(long id)
    {
        var entity = _dbSet.Find(id);
        if (entity == null) throw new NotFoundException("Not found: " + id);

        return entity;
    }

    public PagedResult<AnnualAward> GetPaged(int page, int pageSize)
    {
        if (page < 1)
            page = 1;

        var totalCount = _dbSet.Count();
        var items = _dbSet.OrderBy(e => e.Id).Skip((page - 1)*pageSize).Take(pageSize).ToList();

        return new PagedResult<AnnualAward>(items, totalCount);
    }

    public AnnualAward Update(AnnualAward award)
    {
        try
        {
            DbContext.Update(award);
            DbContext.SaveChanges();
        } 
        catch(DbUpdateException e)
        {
            throw new NotFoundException(e.Message);
        }

        return award;
    }
}
