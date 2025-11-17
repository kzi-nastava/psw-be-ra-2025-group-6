using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using DomainBlog = Explorer.Blog.Core.Domain.Blog;

namespace Explorer.Blog.Infrastructure.Database.Repositories;

public class BlogDbRepository : IBlogRepository
{
    protected readonly BlogContext DbContext;
    private readonly DbSet<DomainBlog> _dbSet;

    public BlogDbRepository(BlogContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<DomainBlog>();
    }

    public List<DomainBlog> GetByUser(long userId)
    {
        return _dbSet.Where(b => b.UserId == userId).ToList();
    }

    public DomainBlog Create(DomainBlog blog)
    {
        _dbSet.Add(blog);
        DbContext.SaveChanges();
        return blog;
    }

    public DomainBlog Update(DomainBlog blog)
    {
        try
        {
            DbContext.Update(blog);
            DbContext.SaveChanges();
        }
        catch (DbUpdateException e)
        {
            throw new NotFoundException(e.Message);
        }
        return blog;
    }

    public PagedResult<DomainBlog> GetPaged(int page, int pageSize)
    {
        var task = _dbSet.GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
    }
}
