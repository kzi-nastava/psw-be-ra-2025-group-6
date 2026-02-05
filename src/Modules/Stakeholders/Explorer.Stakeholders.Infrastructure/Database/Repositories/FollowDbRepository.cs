using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class FollowDbRepository : IFollowRepository
{
    private readonly StakeholdersContext _dbContext;
    private readonly DbSet<Follow> _dbSet;
    private readonly DbSet<User> _users;

    public FollowDbRepository(StakeholdersContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<Follow>();
        _users = _dbContext.Set<User>();
    }

    public Follow Get(long id)
    {
        return _dbSet.FirstOrDefault(f => f.Id == id);
    }

    public Follow Create(Follow entity)
    {
        _dbSet.Add(entity);
        _dbContext.SaveChanges();
        return entity;
    }

    public Follow Update(Follow entity)
    {
        _dbSet.Update(entity);
        _dbContext.SaveChanges();
        return entity;
    }

    public void Delete(long id)
    {
        var entity = Get(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            _dbContext.SaveChanges();
        }
    }

    public void Delete(Follow entity)
    {
        _dbSet.Remove(entity);
        _dbContext.SaveChanges();
    }

    public bool Exists(long followerId, long followedId)
    {
        return _dbSet
            .Any(f => f.FollowerId == followerId && f.FollowedId == followedId);
    }

    public IEnumerable<User> GetFollowers(long userId)
    {
        return _dbSet
            .Where(f => f.FollowedId == userId).AsNoTracking()
            .Join(_dbContext.Users,
                  f => f.FollowerId,
                  u => u.Id,
                  (f, u) => u)
            .Where(u => u.IsActive)
            .ToList();
    }

    public IEnumerable<User> GetFollowing(long userId)
    {
        return _dbSet
            .Where(f => f.FollowerId == userId).AsNoTracking()
            .Join(_dbContext.Users,
                  f => f.FollowedId,
                  u => u.Id,
                  (f, u) => u)
            .Where(u => u.IsActive)
            .ToList();
    }

    public int GetFollowersCount(long userId)
    {
        return _dbSet
            .Where(f => f.FollowedId == userId)
            .Join(_users.Where(u => u.IsActive),
                  f => f.FollowerId,
                  u => u.Id,
                  (f, u) => u)
            .Count();
    }

    public int GetFollowingCount(long userId)
    {
        return _dbSet
            .Where(f => f.FollowerId == userId)
            .Join(_users.Where(u => u.IsActive),
                  f => f.FollowedId,
                  u => u.Id,
                  (f, u) => u)
            .Count();
    }
    public Follow? Find(Expression<Func<Follow, bool>> predicate)
    {
        return _dbSet.FirstOrDefault(predicate);
    }
    public bool IsFollowing(long followerId, long followedId)
    {
        return _dbSet.Any(f => f.FollowerId == followerId && f.FollowedId == followedId);
    }
}
