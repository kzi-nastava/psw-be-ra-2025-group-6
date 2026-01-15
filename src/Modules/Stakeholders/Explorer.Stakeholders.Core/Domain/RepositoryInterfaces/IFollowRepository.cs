using Explorer.BuildingBlocks.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IFollowRepository
{
    Follow Get(long id);
    Follow Create(Follow entity);
    Follow Update(Follow entity);
    void Delete(long id);
    void Delete(Follow entity);
    bool Exists(long followerId, long followedId);
    IEnumerable<User> GetFollowers(long userId);
    IEnumerable<User> GetFollowing(long userId);
    Follow? Find(Expression<Func<Follow, bool>> predicate);
    bool IsFollowing(long followerId, long followedId);
    int GetFollowersCount(long userId);
    int GetFollowingCount(long userId);
}
