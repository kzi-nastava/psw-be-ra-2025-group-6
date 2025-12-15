using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class PublicEntityRequestDbRepository : IPublicEntityRequestRepository
{
    protected readonly ToursContext DbContext;
    private readonly DbSet<PublicEntityRequest> _dbSet;

    public PublicEntityRequestDbRepository(ToursContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<PublicEntityRequest>();
    }

    public PagedResult<PublicEntityRequest> GetPaged(int page, int pageSize)
    {
        var task = _dbSet.GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
    }

    public PublicEntityRequest Get(long id)
    {
        var entity = _dbSet.Find(id);
        if (entity == null) throw new NotFoundException("Not found: " + id);
        return entity;
    }

    public PublicEntityRequest Create(PublicEntityRequest entity)
    {
        _dbSet.Add(entity);
        DbContext.SaveChanges();
        return entity;
    }

    public PublicEntityRequest Update(PublicEntityRequest entity)
    {
        try
        {
            DbContext.Update(entity);
            DbContext.SaveChanges();
        }
        catch (DbUpdateException e)
        {
            throw new NotFoundException(e.Message);
        }
        return entity;
    }

    public List<PublicEntityRequest> GetByAuthor(long authorId)
    {
        return _dbSet.Where(r => r.AuthorId == authorId).ToList();
    }

    public List<PublicEntityRequest> GetPending()
    {
        return _dbSet.Where(r => r.Status == RequestStatus.Pending).ToList();
    }
}
