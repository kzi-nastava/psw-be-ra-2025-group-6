using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Services;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;

public class FollowService : IFollowService
{
    private readonly IFollowRepository _followRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _profileRepository;
    private readonly IMapper _mapper;

    public FollowService(
        IFollowRepository followRepository,
        IUserRepository userRepository,
        IUserProfileRepository profileRepository,   
        IMapper mapper)
    {
        _followRepository = followRepository;
        _userRepository = userRepository;
        _profileRepository = profileRepository;
        _mapper = mapper;
    }

    public FollowDto Follow(long followerId, long followedId)
    {
        var follower = _userRepository.Get(followerId);
        var followed = _userRepository.Get(followedId);

        if (follower == null || !follower.IsActive)
            throw new NotFoundException("Follower not found or inactive");

        if (followed == null || !followed.IsActive)
            throw new NotFoundException("User to follow not found or inactive");

        if (_followRepository.Exists(followerId, followedId))
            throw new ArgumentException("Already following");

        var follow = new Follow(followerId, followedId);
        var created = _followRepository.Create(follow);

        return _mapper.Map<FollowDto>(created);
    }

    public void Unfollow(long followerId, long followedId)
    {
        var follow = _followRepository.Find(f => f.FollowerId == followerId && f.FollowedId == followedId);
        if (follow == null) throw new NotFoundException("Not following");

        _followRepository.Delete(follow);
    }

    public List<UserDto> GetFollowers(long userId)
    {
        var followers = _followRepository.GetFollowers(userId);
        return _mapper.Map<List<UserDto>>(followers);
    }

    public List<UserDto> GetFollowing(long userId)
    {
        var following = _followRepository.GetFollowing(userId);
        return _mapper.Map<List<UserDto>>(following);
    }

    public int GetFollowersCount(long userId)
    {
        return _followRepository.GetFollowersCount(userId);
    }

    public int GetFollowingCount(long userId)
    {
        return _followRepository.GetFollowingCount(userId);
    }

    public bool IsFollowing(long followerId, long followedId)
    {
        return _followRepository.IsFollowing(followerId, followedId);
    }

    public List<UserProfileDto> GetFollowersList(long userId, long currentUserId)
    {
        var followers = _followRepository.GetFollowers(userId);
        var result = new List<UserProfileDto>();

        foreach (var follower in followers)
        {
            try
            {
                var profile = _profileRepository.Get(follower.Id);

                result.Add(new UserProfileDto
                {
                    UserId = follower.Id,
                    Name = profile.Name,
                    Surname = profile.Surname,
                    ProfilePicture = profile.ProfilePicture,
                    Biography = profile.Biography,
                    Quote = profile.Quote,
                    IsFollowedByMe = _followRepository.IsFollowing(currentUserId, follower.Id),
                    Username = follower.Username
                });
            }
            catch (NotFoundException)
            {
                continue;
            }
        }
        return result;
    }

    public List<UserProfileDto> GetFollowingList(long userId, long currentUserId)
    {
        var following = _followRepository.GetFollowing(userId);
        var result = new List<UserProfileDto>();

        foreach (var followedUser in following)
        {
            try
            {
                var profile = _profileRepository.Get(followedUser.Id);

                result.Add(new UserProfileDto
                {
                    UserId = followedUser.Id,
                    Name = profile.Name,
                    Surname = profile.Surname,
                    ProfilePicture = profile.ProfilePicture,
                    Biography = profile.Biography,
                    Quote = profile.Quote,
                    IsFollowedByMe = _followRepository.IsFollowing(currentUserId, followedUser.Id),
                    Username = followedUser.Username
                });
            }
            catch (NotFoundException)
            {
                continue;
            }
        }
        return result;
    }
}
