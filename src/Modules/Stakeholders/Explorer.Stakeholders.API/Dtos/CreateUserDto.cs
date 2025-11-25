namespace Explorer.Stakeholders.API.Dtos;

public class CreateUserDto
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Role { get; set; }       // Administrator, Author, Tourist
    public bool IsActive { get; set; }     // Da li nalog odmah aktivan
}