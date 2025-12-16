namespace Explorer.Stakeholders.API.Dtos;

public enum ProfileResourceTypeDto
{
    Tour = 0,
    Blog = 1
}

public class ProfilePostDto
{
    public long Id { get; set; }
    public long AuthorId { get; set; }
    public string Text { get; set; } = string.Empty;
    public long? ResourceId { get; set; }
    public ProfileResourceTypeDto? ResourceType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
