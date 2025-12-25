using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database.Repositories;

public class TourPurchaseTokenRepository : ITourPurchaseTokenRepository
{
    private readonly PaymentsContext _dbContext;
    private readonly DbSet<TourPurchaseToken> _dbSet;

    public TourPurchaseTokenRepository(PaymentsContext dbContext)
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

    public TourPurchaseToken? GetUnusedByTouristAndTour(long touristId, long tourId)
    {
        return _dbSet.FirstOrDefault(t => t.TouristId == touristId && t.TourId == tourId && !t.IsUsed);
    }
    public TourPurchaseToken Update(TourPurchaseToken token)
    {
        try
        {
            _dbContext.Update(token);
            _dbContext.SaveChanges();
        }
        catch (DbUpdateException e)
        {
            throw new NotFoundException(e.Message);
        }
        return token;
    }
}
