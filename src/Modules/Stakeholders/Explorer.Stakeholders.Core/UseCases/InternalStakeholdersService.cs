using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.UseCases;
public class InternalStakeholdersService : IInternalStakeholderService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IFollowRepository _followRepository;
    private readonly IClubRepository _clubRepository;
    
    public InternalStakeholdersService(
        IUserRepository userRepository, 
        IUserProfileRepository userProfileRepository, 
        IFollowRepository followRepository,
        IClubRepository clubRepository)
    {
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
        _followRepository = followRepository;
        _clubRepository = clubRepository;
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

    public List<long> GetAllTouristIds()
    {
        // Returns PersonIds (not UserIds!)
        var tourists = _userRepository.GetAll()
            .Where(u => u.Role == UserRole.Tourist && u.IsActive)
            .ToList();

        var personIds = new List<long>();
        foreach (var user in tourists)
        {
            try
            {
                var personId = _userRepository.GetPersonId(user.Id);
                personIds.Add(personId);
            }
            catch (KeyNotFoundException)
            {
                // Skip users without Person record
            }
        }

        return personIds;
    }

    public List<(long PersonId, string Username)> GetAllTouristsWithPersonIds()
    {
        var tourists = _userRepository.GetAll()
            .Where(u => u.Role == UserRole.Tourist && u.IsActive)
            .ToList();

        var result = new List<(long PersonId, string Username)>();
        foreach (var user in tourists)
        {
            try
            {
                var personId = _userRepository.GetPersonId(user.Id);
                result.Add((personId, user.Username));
            }
            catch (KeyNotFoundException)
            {
                // Skip users without Person record
            }
        }

        return result;
    }

    public List<(long ClubId, string ClubName)> GetAllClubs()
    {
        var clubs = _clubRepository.GetAll();
        return clubs.Select(c => (c.Id, c.Name)).ToList();
    }
}

