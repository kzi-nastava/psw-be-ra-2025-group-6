using Explorer.Stakeholders.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Services;

public interface IFollowService
{
    FollowDto Follow(long followerId, long followedId);
    void Unfollow(long followerId, long followedId);
    List<UserDto> GetFollowers(long userId);
    List<UserDto> GetFollowing(long userId);
    int GetFollowersCount(long userId);
    int GetFollowingCount(long userId);
    bool IsFollowing(long followerId, long followedId);
    List<UserProfileDto> GetFollowersList(long userId, long currentUserId);
    List<UserProfileDto> GetFollowingList(long userId, long currentUserId);
}
