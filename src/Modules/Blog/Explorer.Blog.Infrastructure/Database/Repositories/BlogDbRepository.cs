using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.Blog.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Blog.Infrastructure.Database.Repositories;

public class BlogDbRepository : IBlogRepository
{
    protected readonly BlogContext DbContext;
    private readonly DbSet<BlogPost> _dbSet;

    public BlogDbRepository(BlogContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<BlogPost>();
    }

    public List<BlogPost> GetByUser(long userId)
    {
        return _dbSet.Where(b => b.UserId == userId).ToList();
    }

    public BlogPost Create(BlogPost blog)
    {
        _dbSet.Add(blog);
        DbContext.SaveChanges();
        return blog;
    }

    public BlogPost Update(BlogPost blog)
    {
        var existingBlog = _dbSet.Include(b => b.Votes).FirstOrDefault(b => b.Id == blog.Id);
        if (existingBlog == null)
            throw new NotFoundException($"Blog with Id {blog.Id} not found.");

        _dbSet.Update(blog);
        DbContext.Entry(existingBlog).CurrentValues.SetValues(blog);

        var existingVotes = existingBlog.Votes.ToList();
        existingVotes.ForEach(v => existingBlog.Votes.ToList().Remove(v));
        foreach (var vote in blog.Votes)
        {
            existingBlog.Votes.ToList().Add(vote);
        }

        DbContext.SaveChanges();
        return blog;
    }

    public PagedResult<BlogPost> GetPaged(int page, int pageSize)
    {
        var task = _dbSet.GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
    }

    public BlogPost GetById(long id)
    {
        return _dbSet
            .Include(b => b.Votes)
            .Include(b => b.Comments)
            .FirstOrDefault(b => b.Id == id);
    }

    public void Delete(BlogPost blog)
    {
        _dbSet.Remove(blog);
        DbContext.SaveChanges();
    }

    public List<BlogPost> GetAll()
    {
        return _dbSet
            .Include(b => b.Votes)
            .Include(b => b.Comments)
            .ToList();
    }
}
