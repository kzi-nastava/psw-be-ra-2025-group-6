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
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IPersonRepository personRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _personRepository = personRepository;
            _mapper = mapper;
        }

        public UserDto CreateUser(CreateUserDto dto)
        {

            var role = Enum.Parse<UserRole>(dto.Role, true);

            // Kreiraj User entitet
            var user = new User(dto.Username, dto.Password, role, dto.IsActive);
            _userRepository.Create(user);

            // Kreiraj Person entitet i poveži sa User.Id
            var person = new Person(user.Id, dto.Name, dto.Surname, dto.Email);
            _personRepository.Create(person);

            // Mapiraj User u UserDto
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
    }
}
