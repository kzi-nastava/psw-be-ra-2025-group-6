using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Blog.Infrastructure.Database.Repositories;
public class CommentLikeDbRepository : ICommentLikeRepository
{
    protected readonly BlogContext DbContext;
    private readonly DbSet<CommentLike> _dbSet;

    public CommentLikeDbRepository(BlogContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<CommentLike>();
    }

    public bool Toggle(long blogId, long commentId, long userId)
    {
        var existing = _dbSet
            .FirstOrDefault(x => x.BlogId == blogId && x.CommentId == commentId && x.UserId == userId);

        if (existing != null)
        {
            _dbSet.Remove(existing);
            DbContext.SaveChanges();
            return false;
        }

        var like = new CommentLike(blogId, commentId, userId);
        _dbSet.Add(like);
        DbContext.SaveChanges();
        return true;
    }

    public int CountLikes(long blogId, long commentId)
    {
        return _dbSet.Count(like => like.BlogId == blogId && like.CommentId == commentId);
    }

    public bool IsLikedByUser(long blogId, long commentId, long userId)
    {
        return _dbSet.Any(like => like.BlogId == blogId && like.CommentId == commentId && like.UserId == userId);
    }
}

