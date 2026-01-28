using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

public class Achievement : Entity
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string IconUrl { get; private set; }
    public string Category { get; private set; }
    public int? Threshold { get; private set; }

    public List<UserRole> Role { get; set; } = new List<UserRole>();

    public ICollection<UserProfile> UserProfiles { get; private set; }
        = new List<UserProfile>();

    public Achievement(
        string code,
        string name,
        string description,
        string iconUrl,
        string category,
        int? threshold)
    {
        Code = code;
        Name = name;
        Description = description;
        IconUrl = iconUrl;
        Category = category;
        Threshold = threshold;

        Validate();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Code))
            throw new ArgumentException("Achievement code is required.");

        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Achievement name is required.");

        if (string.IsNullOrWhiteSpace(Description))
            throw new ArgumentException("Achievement description is required.");

        if (string.IsNullOrWhiteSpace(Category))
            throw new ArgumentException("Achievement category is required.");

    }

    public void Update(
        string name,
        string description,
        string iconUrl,
        string category,
        int? threshold)
    {
        Name = name;
        Description = description;
        IconUrl = iconUrl;
        Category = category;
        Threshold = threshold;

        Validate();
    }
}

