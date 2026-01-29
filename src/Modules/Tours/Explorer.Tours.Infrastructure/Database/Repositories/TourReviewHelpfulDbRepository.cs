using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourReviewHelpfulDbRepository : ITourReviewHelpfulRepository
{
    private readonly ToursContext _dbContext;
    private readonly DbSet<TourReviewHelpfulVote> _dbSet;

    public TourReviewHelpfulDbRepository(ToursContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<TourReviewHelpfulVote>();
    }

    public bool Exists(long reviewId, long userId)
    {
        return _dbSet.Any(v => v.ReviewId == reviewId && v.UserId == userId);
    }

    public void Add(TourReviewHelpfulVote vote)
    {
        // Try to insert; if unique constraint is violated by concurrent insert,
        // DbUpdateException will be thrown by SaveChanges. Treat that as "already exists".
        _dbSet.Add(vote);
        try
        {
            _dbContext.SaveChanges();
        }
        catch (DbUpdateException)
        {
            // swallow: another concurrent request already inserted the same (ReviewId,UserId)
            // DB has unique index so duplicate insert will fail — that's fine for idempotency.
        }
    }

    public void RemoveByReviewAndUser(long reviewId, long userId)
    {
        var existing = _dbSet.FirstOrDefault(v => v.ReviewId == reviewId && v.UserId == userId);
        if (existing == null) return;
        _dbSet.Remove(existing);
        _dbContext.SaveChanges();
    }

    public int CountByReview(long reviewId)
    {
        return _dbSet.Count(v => v.ReviewId == reviewId);
    }
}