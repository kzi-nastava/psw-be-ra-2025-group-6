using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;
public class InternalStakeholdersService : IInternalStakeholderService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _profileRepository;

    public InternalStakeholdersService(IUserRepository userRepository, IUserProfileRepository profileRepository)
    {
        _userRepository = userRepository;
        _profileRepository = profileRepository;
    }

    public string GetUsername(long userId)
    {
        var user = _userRepository.GetById(userId);
        if (user == null) throw new Exception("User not found.");

        return user.Username;
    }

    public string GetProfilePicture(long userId)
    {
        try
        {
            return _profileRepository.Get(userId).ProfilePicture ?? "";
        }
        catch (NotFoundException)
        {
            return "";
        }
    }
}

