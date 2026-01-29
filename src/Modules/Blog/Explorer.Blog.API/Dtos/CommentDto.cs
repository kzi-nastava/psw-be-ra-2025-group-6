namespace Explorer.Blog.API.Dtos;

public class CommentDto
{
    public long Id { get; set; }
    public long BlogId { get; set; }
    public long UserId { get; set; }
    public string AuthorName { get; set; }
    public string AuthorProfilePicture { get; set; }
    public string Text{ get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public int LikeCount { get; set; }
    public bool IsLikedByMe { get; set; }
    public bool IsReportedByMe { get; set; }

    public bool IsHidden { get; set; }
}
