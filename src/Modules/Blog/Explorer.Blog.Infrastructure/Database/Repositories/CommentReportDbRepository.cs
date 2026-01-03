using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Blog.Infrastructure.Database.Repositories;
public class CommentReportDbRepository : ICommentReportRepository
{
    protected readonly BlogContext DbContext;
    private readonly DbSet<CommentReport> _dbSet;

    public CommentReportDbRepository(BlogContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<CommentReport>();
    }

    public bool Exists(long blogId, long commentId, long userId)
    {
        return _dbSet.Any(x => x.BlogId == blogId && x.CommentId == commentId && x.UserId == userId);
    }
    public CommentReport Create(CommentReport report)
    {
        if (Exists(report.BlogId, report.CommentId, report.UserId))
        {
            throw new InvalidOperationException("User already reported this comment.");
        }

        _dbSet.Add(report);
        DbContext.SaveChanges();
        return report;
    }

    public CommentReport Get(long blogId, long commentId, long userId)
    {
        return _dbSet.FirstOrDefault(x => x.BlogId == blogId && x.CommentId == commentId && x.UserId == userId);
    }

    public CommentReport GetById(long id)
    {
        return _dbSet.FirstOrDefault(x => x.Id == id);
    }

    public IEnumerable<CommentReport> GetOpen(int skip, int take)
    {
        return _dbSet
            .Where(r => r.ReportStatus == AdminReportStatus.OPEN)
            .OrderBy(r => r.CreatedAt)
            .Skip(skip)
            .Take(take)
            .AsNoTracking()
            .ToList();
    }

    public int CountOpen()
    {
        return _dbSet.Count(x => x.ReportStatus == AdminReportStatus.OPEN);
    }

    public CommentReport Update(CommentReport report)
    {
        _dbSet.Update(report);
        DbContext.SaveChanges();
        return report;
    }
}

