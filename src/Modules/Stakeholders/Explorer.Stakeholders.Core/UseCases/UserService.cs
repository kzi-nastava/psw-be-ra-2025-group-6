using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Services;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public UserDto CreateUser(CreateUserDto dto)
        {
            var role = Enum.Parse<UserRole>(dto.Role, true);

            var user = new User(dto.Username, dto.Password, role, dto.IsActive);

            _userRepository.Create(user);

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
