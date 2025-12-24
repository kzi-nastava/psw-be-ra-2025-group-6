using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class KeyPointDbRepository : IKeyPointRepository
{
    private readonly ToursContext _dbContext;
    private readonly DbSet<KeyPoint> _dbSet;

    public KeyPointDbRepository(ToursContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<KeyPoint>();
    }

    public List<KeyPoint> GetPublicKeyPoints()
    {
        return _dbSet.Where(kp => kp.IsPublic).ToList();
    }
}
