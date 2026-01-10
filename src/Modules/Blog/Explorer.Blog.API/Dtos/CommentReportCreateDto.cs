namespace Explorer.Blog.API.Dtos;
public class CommentReportCreateDto
{
    public ReportTypeDto Reason { get; set; }
    public string? AdditionalInfo { get; set; }
}