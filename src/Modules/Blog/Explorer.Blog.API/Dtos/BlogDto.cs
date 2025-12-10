namespace Explorer.Blog.API.Dtos;

public class BlogDto
{
    public int Id { get; set; }
    public long UserId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Images { get; set; }
    public List<CommentDto> Comments { get; set; } = new();
    public BlogStatusDto Status { get; set; }
    public DateTime? LastModifiedAt { get; set; }

}
