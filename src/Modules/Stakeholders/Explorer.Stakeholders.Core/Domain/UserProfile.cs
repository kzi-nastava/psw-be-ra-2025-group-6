using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

public class UserProfile : Entity
{
    public long UserId { get; init; }
    public string Name { get; private set; }
    public string Surname { get; private set; }
    public string ProfilePicture { get; private set; }
    public string Biography { get; private set; }
    public string Quote { get; private set; }
    public ICollection<Achievement> Achievements { get; private set; }
        = new List<Achievement>();

    public UserProfile(long userId, string name, string surname, string profilePicture, string biography, string quote)
    {
        UserId = userId;
        Name = name;
        Surname = surname;
        ProfilePicture = profilePicture;
        Biography = biography;
        Quote = quote;
        Validate();
    }

    private void Validate()
    {
        if (UserId == 0) throw new ArgumentException("Invalid UserId");
        if (string.IsNullOrWhiteSpace(Name)) throw new ArgumentException("Invalid Name");
        if (string.IsNullOrWhiteSpace(Surname)) throw new ArgumentException("Invalid Surname");
    }

    public void Update(string name, string surname, string profilePicture, string biography, string quote)
    {
        Name = name;
        Surname = surname;
        ProfilePicture = profilePicture;
        Biography = biography;
        Quote = quote;
        Validate();
    }

}
