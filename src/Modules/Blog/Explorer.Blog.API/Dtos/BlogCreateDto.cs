namespace Explorer.Blog.API.Dtos;

public class BlogCreateDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public BlogStatusDto Status { get; set; }
    public List<BlogContentItemDto> ContentItems { get; set; } = new();
    //public string? Location { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Region { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
