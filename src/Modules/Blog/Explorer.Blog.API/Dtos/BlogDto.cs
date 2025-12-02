namespace Explorer.Blog.API.Dtos;

public class BlogDto
{
    public int Id { get; set; }
    public long UserId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Images { get; set; }
    public BlogStatusDto Status { get; set; }
}
