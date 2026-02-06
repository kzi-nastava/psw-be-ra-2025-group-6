using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Encounters.API.Internal;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;

public class AuthenticationService : IAuthenticationService
{
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IUserRepository _userRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IInternalLeaderboardService _leaderboardService;

    public AuthenticationService(
        IUserRepository userRepository,
        IPersonRepository personRepository,
        ITokenGenerator tokenGenerator,
        IUserProfileRepository userProfileRepository,
        IInternalLeaderboardService leaderboardService)
    {
        _tokenGenerator = tokenGenerator;
        _userRepository = userRepository;
        _personRepository = personRepository;
        _userProfileRepository = userProfileRepository;
        _leaderboardService = leaderboardService;
    }

    public AuthenticationTokensDto Login(CredentialsDto credentials)
    {
        var user = _userRepository.GetActiveByName(credentials.Username);
        if (user == null || credentials.Password != user.Password)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        long personId;
        try
        {
            personId = _userRepository.GetPersonId(user.Id);
        }
        catch (KeyNotFoundException)
        {
            personId = 0;
        }
        return _tokenGenerator.GenerateAccessToken(user, personId);
    }

    public AuthenticationTokensDto RegisterTourist(AccountRegistrationDto account)
    {
        if (_userRepository.Exists(account.Username))
            throw new EntityValidationException("Provided username already exists.");

        var user = _userRepository.Create(new User(account.Username, account.Password, UserRole.Tourist, true));
        var person = _personRepository.Create(new Person(user.Id, account.Name, account.Surname, account.Email));
        _userProfileRepository.Create(new UserProfile(user.Id, account.Name, account.Surname, "", "", ""));

        // ✨ Automatically create leaderboard entry for new tourist
        try
        {
            _leaderboardService.CreateLeaderboardEntryForNewUser(user.Id, account.Username);
        }
        catch
        {
            // Leaderboard creation is not critical - proceed with registration
        }

        return _tokenGenerator.GenerateAccessToken(user, person.Id);
    }
}
