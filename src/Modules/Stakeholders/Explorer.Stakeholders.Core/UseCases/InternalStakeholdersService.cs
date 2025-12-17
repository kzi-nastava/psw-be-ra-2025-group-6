using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;
public class InternalStakeholdersService : IInternalStakeholderService
{
    private readonly IUserRepository _userRepository;

    public InternalStakeholdersService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public string GetUsername(long userId)
    {
        var user = _userRepository.GetById(userId);
        if (user == null) throw new Exception("User not found.");

        return user.Username;
    }
}

