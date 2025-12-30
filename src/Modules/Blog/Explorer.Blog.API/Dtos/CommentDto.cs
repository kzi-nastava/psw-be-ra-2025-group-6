namespace Explorer.Blog.API.Dtos;

public class CommentDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string AuthorName { get; set; }
    public string AuthorProfilePicture { get; set; }
    public string Text{ get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
}
