namespace Explorer.Blog.API.Dtos;
public class CommentReportDto
{
    public int Id { get; set; }
    public long BlogId { get; set; }
    public long CommentId { get; set; }
    public long UserId { get; set; }
    public ReportTypeDto Reason { get; set; }
    public string? AdditionalInfo { get; set; }
    public DateTime CreatedAt { get; set; }
    public AdminReportStatusDto ReportStatus { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public long? ReviewerId { get; set; }
    public string? AdminNote { get; set; }
    public long CommentAuthorId { get; set; }
    public string CommentAuthorName { get; set; } = "";
    public string CommentText { get; set; } = "";
    public DateTime CommentCreatedAt { get; set; }
}

