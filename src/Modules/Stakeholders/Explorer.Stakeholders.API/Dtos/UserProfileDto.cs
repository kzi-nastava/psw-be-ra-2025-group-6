namespace Explorer.Stakeholders.API.Dtos;

public class UserProfileDto
{
    public long UserId { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string ProfilePicture { get; set; }
    public string Biography { get; set; }
    public string Quote { get; set; }
}
