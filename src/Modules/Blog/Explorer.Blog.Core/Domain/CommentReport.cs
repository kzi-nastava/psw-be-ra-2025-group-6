using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Blog.Core.Domain;

public class CommentReport : Entity
{
    public long BlogId { get; private set; }
    public long CommentId { get; private set; }
    public long UserId { get; private set; }
    public ReportType Reason { get; private set; }
    public string? AdditionalInfo { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public AdminReportStatus ReportStatus { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public long? ReviewerId { get; private set; }
    public string? AdminNote { get; private set; }

    public CommentReport() {}

    public CommentReport(long blogId, long commentId, long userId, ReportType reason, string? additionalInfo,
        AdminReportStatus reportStatus)
    {
        if (blogId <= 0) throw new ArgumentException("Invalid blog.");
        if (commentId <= 0) throw new ArgumentException("Invalid comment.");
        if (UserId <= 0) throw new ArgumentException("Invalid user.");

        BlogId = blogId;
        CommentId = commentId;
        UserId = userId;
        Reason = reason;
        AdditionalInfo = string.IsNullOrWhiteSpace(additionalInfo) ? null : additionalInfo.Trim();
        CreatedAt = DateTime.UtcNow;
        ReportStatus = AdminReportStatus.OPEN;
    }

    public void Approve(long adminId, string? note = null)
        => Review(adminId, AdminReportStatus.APPROVED, note);

    public void Dismiss(long adminId, string? note = null)
        => Review(adminId, AdminReportStatus.DISMISSED, note);

    private void Review(long adminId, AdminReportStatus newStatus, string? note)
    {
        if (adminId <= 0) throw new ArgumentException("Invalid admin.");
        if (ReportStatus != AdminReportStatus.OPEN)
            throw new InvalidOperationException("Report is already reviewed.");

        ReportStatus = newStatus;
        ReviewedAt = DateTime.UtcNow;
        ReviewerId = adminId;
        AdminNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
    }
}
