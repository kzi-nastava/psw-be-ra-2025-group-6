using Explorer.BuildingBlocks.Core.Exceptions;
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

    public AuthenticationService(IUserRepository userRepository, IPersonRepository personRepository, ITokenGenerator tokenGenerator, IUserProfileRepository userProfileRepository)
    {
        _tokenGenerator = tokenGenerator;
        _userRepository = userRepository;
        _personRepository = personRepository;
        _userProfileRepository = userProfileRepository;
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
            throw new AlreadyExistsException("Provided username already exists.", "username");

        User user;
        try
        {
            user = new User(account.Username, account.Password, UserRole.Tourist, true);
        }
        catch (ArgumentException ex)
        {
            throw BuildUserValidationException(ex);
        }

        user = _userRepository.Create(user);

        Person person;
        try
        {
            person = new Person(user.Id, account.Name, account.Surname, account.Email);
        }
        catch (ArgumentException ex)
        {
            throw BuildPersonValidationException(ex);
        }

        person = _personRepository.Create(person);

        try
        {
            _userProfileRepository.Create(new UserProfile(user.Id, account.Name, account.Surname, "", "", ""));
        }
        catch (ArgumentException ex)
        {
            throw BuildPersonValidationException(ex);
        }

        return _tokenGenerator.GenerateAccessToken(user, person.Id);
    }

    private static RequestValidationException BuildUserValidationException(ArgumentException ex)
    {
        var (field, message) = ex.Message switch
        {
            "Invalid Name" => ("username", "Username is required."),
            "Invalid Surname" => ("password", "Password is required."),
            _ => ("general", ex.Message)
        };

        return new RequestValidationException(new[] { new ValidationError(field, message) });
    }

    private static RequestValidationException BuildPersonValidationException(ArgumentException ex)
    {
        var (field, message) = ex.Message switch
        {
            "Invalid Name" => ("name", "Name is required."),
            "Invalid Surname" => ("surname", "Surname is required."),
            "Invalid Email" => ("email", "Email is invalid."),
            _ => ("general", ex.Message)
        };

        return new RequestValidationException(new[] { new ValidationError(field, message) });
    }
}
