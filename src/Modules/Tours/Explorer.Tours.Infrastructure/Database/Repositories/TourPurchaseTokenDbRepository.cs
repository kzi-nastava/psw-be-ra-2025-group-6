using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourPurchaseTokenDbRepository : ITourPurchaseTokenRepository
{
    private readonly ToursContext _dbContext;
    private readonly DbSet<TourPurchaseToken> _dbSet;

    public TourPurchaseTokenDbRepository(ToursContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<TourPurchaseToken>();
    }

    public List<TourPurchaseToken> GetByTouristId(long touristId)
    {
        return _dbSet.Where(t => t.TouristId == touristId).ToList();
    }

    public TourPurchaseToken Create(TourPurchaseToken token)
    {
        _dbSet.Add(token);
        _dbContext.SaveChanges();
        return token;
    }

    public List<TourPurchaseToken> CreateBulk(List<TourPurchaseToken> tokens)
    {
        _dbSet.AddRange(tokens);
        _dbContext.SaveChanges();
        return tokens;
    }

    public TourPurchaseToken GetByTouristAndTour(long touristId, long tourId)
    {
        return _dbSet.FirstOrDefault(t => t.TouristId == touristId && t.TourId == tourId);
    }
}