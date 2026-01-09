using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public.Administration;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration;

[Authorize(Policy = "administratorPolicy")]
[Route("api/admin/comment-reports")]
[ApiController]
public class CommentReportController : ControllerBase
{
    private readonly IBlogService _blogService;

    public CommentReportController(IBlogService blogService)
    {
        _blogService = blogService;
    }

    [HttpGet("open")]
    public ActionResult GetByReportStatus([FromQuery] AdminReportStatusDto status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = _blogService.GetByReportStatus(status, page, pageSize);
        return Ok(result);
    }

    [HttpPatch("{reportId:long}/approve")]
    public IActionResult Approve(long reportId, [FromBody] CommentReportReviewDto dto)
    {
        var adminId = User.PersonId();
        _blogService.ApproveCommentReport(reportId, adminId, dto.Note);
        return NoContent();
    }

    [HttpPatch("{reportId:long}/dismiss")]
    public IActionResult Dismiss(long reportId, [FromBody] CommentReportReviewDto dto)
    {
        var adminId = User.PersonId();
        _blogService.DismissCommentReport(reportId, adminId, dto.Note);
        return NoContent();
    }
}
