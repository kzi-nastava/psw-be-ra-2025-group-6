using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Services;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IMapper _mapper;

        public UserService(
            IUserRepository userRepository, 
            IPersonRepository personRepository, 
            IUserProfileRepository userProfileRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _personRepository = personRepository;
            _userProfileRepository = userProfileRepository;
            _mapper = mapper;
        }

        public UserDto CreateUser(CreateUserDto dto)
        {
            var role = Enum.Parse<UserRole>(dto.Role, true);

            var user = new User(dto.Username, dto.Password, role, dto.IsActive);
            Console.WriteLine($"[USER CREATE] Creating user with username: {dto.Username}");
            
            _userRepository.Create(user);
            Console.WriteLine($"[USER CREATE] User created with Id: {user.Id}");

            var person = new Person(user.Id, dto.Name, dto.Surname, dto.Email);
            _personRepository.Create(person);
            Console.WriteLine($"[USER CREATE] Person created with Id: {person.Id}, UserId: {person.UserId}");

            try
            {
                var userProfile = new UserProfile(user.Id, dto.Name, dto.Surname, "", "", "");
                _userProfileRepository.Create(userProfile);
                Console.WriteLine($"[USER CREATE] UserProfile created with Id: {userProfile.Id}, UserId: {userProfile.UserId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[USER CREATE] Failed to create UserProfile: {ex.Message}");
            }

            return _mapper.Map<UserDto>(user);
        }

        public IEnumerable<UserDto> GetAllUsers()
        {
            var users = _userRepository.GetAll();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public void BlockUser(long userId)
        {
            var user = _userRepository.GetById(userId);
            if (user == null) throw new Exception("User not found");

            user.Block();
            _userRepository.Update(user);
        }

        public UserDto GetUser(long userId)
        {
            var user = _userRepository.Get(userId);
            return _mapper.Map<UserDto>(user);
        }

        public List<UserSearchResultDto> SearchUsers(string searchTerm, int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<UserSearchResultDto>();

            var users = _userRepository.SearchByUsername(searchTerm, limit);
            var results = new List<UserSearchResultDto>();

            foreach (var user in users)
            {
                var personId = _userRepository.GetPersonId(user.Id);
                var person = _personRepository.GetById(personId);
                
                results.Add(new UserSearchResultDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Name = $"{person.Name} {person.Surname}",
                    Email = person.Email
                });
            }

            return results;
        }
    }
}
