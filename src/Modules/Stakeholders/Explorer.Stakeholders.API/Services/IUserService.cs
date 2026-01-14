using Explorer.Stakeholders.API.Dtos;
namespace Explorer.Stakeholders.API.Services
{
    public interface IUserService
    {
        // Kreira novog korisnika, vraća DTO za frontend
        UserDto CreateUser(CreateUserDto dto);

        // Vraća listu svih korisnika
        IEnumerable<UserDto> GetAllUsers();

        UserDto GetUser(long userId);

        // Blokira korisnika po ID-u
        void BlockUser(long userId);

    }
}
