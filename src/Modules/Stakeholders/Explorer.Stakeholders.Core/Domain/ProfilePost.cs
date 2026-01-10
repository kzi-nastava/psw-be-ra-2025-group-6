using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

public enum ProfileResourceType
{
    Tour = 0,
    Blog = 1
}

public class ProfilePost : Entity
{
    public long AuthorId { get; private set; }
    public string Text { get; private set; }
    public long? ResourceId { get; private set; }
    public ProfileResourceType? ResourceType { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private ProfilePost() { Text = string.Empty; }

    public ProfilePost(long authorId, string text, ProfileResourceType? resourceType, long? resourceId)
    {
        AuthorId = authorId;
        Text = text;
        ResourceType = resourceType;
        ResourceId = resourceId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
        Validate();
    }

    public void Update(string text, ProfileResourceType? resourceType, long? resourceId)
    {
        Text = text;
        ResourceType = resourceType;
        ResourceId = resourceId;
        UpdatedAt = DateTime.UtcNow;
        Validate();
    }

    private void Validate()
    {
        if (AuthorId <= 0) throw new ArgumentException("Invalid AuthorId");
        if (string.IsNullOrWhiteSpace(Text)) throw new ArgumentException("Text is required");
        if (Text.Length > 280) throw new ArgumentException("Text exceeds 280 characters");

        if (ResourceType.HasValue != ResourceId.HasValue)
            throw new ArgumentException("ResourceId and ResourceType must both be provided or omitted");

        if (ResourceType.HasValue && ResourceId.GetValueOrDefault() <= 0)
            throw new ArgumentException("Invalid ResourceId");
    }
}
