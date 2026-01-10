using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;
public class InternalStakeholdersService : IInternalStakeholderService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IFollowRepository _followRepository;
    public InternalStakeholdersService(IUserRepository userRepository, IUserProfileRepository userProfileRepository, IFollowRepository followRepository)
    {
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
        _followRepository = followRepository;
    }

    public string GetUsername(long userId)
    {
        var user = _userRepository.GetById(userId);
        if (user == null) throw new Exception("User not found.");

        return user.Username;
    }

    public string GetProfilePicture(long userId)
    {
        var profile = _userProfileRepository.Get(userId);
        return profile?.ProfilePicture ?? "";
    }

    public List<long> GetFollowedIds(long followerId)
    {
        var followedUsers = _followRepository.GetFollowing(followerId);
        return followedUsers.Select(u => (long)u.Id).ToList();
    }
}

