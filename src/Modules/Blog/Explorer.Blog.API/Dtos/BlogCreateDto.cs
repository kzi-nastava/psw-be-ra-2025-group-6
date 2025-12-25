namespace Explorer.Blog.API.Dtos;

public class BlogCreateDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public BlogStatusDto Status { get; set; }
    public List<BlogContentItemDto> ContentItems { get; set; } = new();
    public string? Location { get; set; }
}
