using Explorer.Stakeholders.API.Dtos;
using System.Collections.Generic;

namespace Explorer.Stakeholders.API.Services
{
    public interface IUserService
    {
        // Kreira novog korisnika, vraća DTO za frontend
        UserDto CreateUser(CreateUserDto dto);

        // Vraća listu svih korisnika
        IEnumerable<UserDto> GetAllUsers();

        // Blokira korisnika po ID-u
        void BlockUser(long userId);

    }
}
