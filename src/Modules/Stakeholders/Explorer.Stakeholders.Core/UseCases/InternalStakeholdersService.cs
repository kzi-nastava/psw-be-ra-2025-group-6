using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;
public class InternalStakeholdersService : IInternalStakeholderService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _userProfileRepository;

    public InternalStakeholdersService(IUserRepository userRepository, IUserProfileRepository userProfileRepository)
    {
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
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
}

