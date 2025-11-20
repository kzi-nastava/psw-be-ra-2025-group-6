using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;

namespace Explorer.Stakeholders.Core.Domain;

public class Profile : Entity
{
    public string Name { get; private set; }
    public string Surname { get; private set; }
    public string? Biography { get; private set; }
    public string? Motto { get; private set; }
    public string? ProfilePictureUrl { get; private set; }
    public long PersonId { get; private set; }

    public Profile(string name, string surname, long personId, string? biography, string? motto, string? profilePictureUrl)
    {
        Name = name;
        Surname = surname;
        Biography = biography;
        Motto = motto;
        ProfilePictureUrl = profilePictureUrl;
        PersonId = personId;

        Validate();
    }

    public void Update(string name, string surname, string? biography, string? motto, string? profilePictureUrl)
    {
        Name = name;
        Surname = surname;
        Biography = biography;
        Motto = motto;
        ProfilePictureUrl = profilePictureUrl;

        Validate();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name)) throw new EntityValidationException("Name cannot be empty.");
        if (string.IsNullOrWhiteSpace(Surname)) throw new EntityValidationException("Surname cannot be empty.");
        if (PersonId == 0) throw new EntityValidationException("PersonId must be set.");
        if (Biography?.Length > 250) throw new EntityValidationException("Biography cannot be longer than 250 characters.");
        if (Motto?.Length > 250) throw new EntityValidationException("Motto cannot be longer than 250 characters.");
    }
}
