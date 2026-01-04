using Explorer.API.Controllers;
using Explorer.API.Controllers.Administrator.Administration;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public.Administration;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;
using System.Text.Json;

namespace Explorer.Blog.Tests.Integration;

[Collection("Sequential")]
public class CommentCommandTests : BaseBlogIntegrationTest
{
    public CommentCommandTests(BlogTestFactory factory) : base(factory) { }

    private BlogController CreateController(IServiceScope scope, long userId)
    {
        var blogService = scope.ServiceProvider.GetRequiredService<IBlogService>();
        var controller = new BlogController(blogService);

        var claims = new[]
        {
            new Claim("personId", userId.ToString()),
            new Claim("PersonId", userId.ToString()),
            new Claim("id", userId.ToString()),
            new Claim("userId", userId.ToString()),
            new Claim("sub", userId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        };

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"))
            }
        };

        return controller;
    }

    private CommentReportController CreateAdminReportController(IServiceScope scope, long adminId)
    {
        var blogService = scope.ServiceProvider.GetRequiredService<IBlogService>();
        var controller = new CommentReportController(blogService);

        var claims = new List<Claim>
        {
            new Claim("personId", adminId.ToString()),
            new Claim("PersonId", adminId.ToString()),
            new Claim("id", adminId.ToString()),
            new Claim("userId", adminId.ToString()),
            new Claim("sub", adminId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, adminId.ToString()),
            new Claim(ClaimTypes.Role, "administrator"),
            new Claim("role", "administrator"),
        };

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"))
            }
        };

        return controller;
    }

    private void EnsureBlogExists(BlogContext dbContext, long blogId, long userId)
    {
        if (dbContext.Blogs.Any(b => b.Id == blogId)) return;

        var blog = new Explorer.Blog.Core.Domain.BlogPost(
            userId,
            "Test blog",
            "Opis",
            new List<string>(),
            BlogStatus.POSTED
        );

        typeof(Explorer.Blog.Core.Domain.BlogPost)
            .GetProperty("Id")!
            .SetValue(blog, blogId);

        dbContext.Blogs.Add(blog);
        dbContext.SaveChanges();
    }




    [Fact]
    public void AddComment_creates_comment_in_db()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, userId: -11);
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        const long blogId = -1;
        EnsureBlogExists(dbContext, -1, -11);

        var initialCount = dbContext.Comments.Count(c => c.BlogId == blogId);

        var dto = new CommentCreateDto { Text = "Novi komentar iz testa" };

        var result = controller.AddComment(blogId, dto) as OkObjectResult;
        result.ShouldNotBeNull();

        var created = result.Value as CommentDto;
        created.ShouldNotBeNull();
        created.Text.ShouldBe(dto.Text);
        created.UserId.ShouldBe(-11);

        dbContext.Comments.Count(c => c.BlogId == blogId).ShouldBe(initialCount + 1);

        var stored = dbContext.Comments
            .Where(c => c.BlogId == blogId)
            .OrderByDescending(c => c.CreatedAt)
            .First();

        stored.Text.ShouldBe(dto.Text);
        stored.UserId.ShouldBe(-11);
    }

    [Fact]
    public void EditComment_updates_text_when_author()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, userId: -11);
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        const long blogId = -1;
        EnsureBlogExists(dbContext, -1, -11);

        (controller.AddComment(blogId, new CommentCreateDto { Text = "Stari tekst" }) as OkObjectResult)
            .ShouldNotBeNull();

        var comment = dbContext.Comments
            .Where(c => c.BlogId == blogId && c.UserId == -11 && c.Text == "Stari tekst")
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new { c.Id, c.CreatedAt })
            .First();

        var dto = new CommentEditDto
        {
            CreatedAt = comment.CreatedAt,
            Text = "Izmenjen tekst komentara"
        };

        var actionResult = controller.EditComment(blogId, comment.Id, dto) as OkResult;
        actionResult.ShouldNotBeNull();

        var stored = dbContext.Comments.First(c => c.Id == comment.Id);
        stored.Text.ShouldBe(dto.Text);
        stored.LastUpdatedAt.ShouldNotBeNull();
    }

    [Fact]
    public void EditComment_fails_when_different_user()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        const long blogId = -1;
        EnsureBlogExists(dbContext, -1, -11);

        var controllerAuthor = CreateController(scope, userId: -12);
        (controllerAuthor.AddComment(blogId, new CommentCreateDto { Text = "Tekst komentara" }) as OkObjectResult)
            .ShouldNotBeNull();

        var commentId = dbContext.Comments
            .Where(c => c.BlogId == blogId && c.UserId == -12 && c.Text == "Tekst komentara")
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => c.Id)
            .First();

        var controllerOther = CreateController(scope, userId: -11);

        Should.Throw<InvalidOperationException>(() =>
            controllerOther.EditComment(blogId, commentId, new CommentEditDto
            {
                Text = "Pokusaj izmene"
            }));
    }

    [Fact]
    public void DeleteComment_removes_comment()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, userId: -11);
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        const long blogId = -1;
        EnsureBlogExists(dbContext, -1, -11);

        (controller.AddComment(blogId, new CommentCreateDto { Text = "Tekst za brisanje" }) as OkObjectResult)
            .ShouldNotBeNull();

        var commentId = dbContext.Comments
            .Where(c => c.BlogId == blogId && c.UserId == -11 && c.Text == "Tekst za brisanje")
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => c.Id)
            .First();

        (controller.DeleteComment(blogId, commentId) as NoContentResult).ShouldNotBeNull();

        dbContext.Comments.Any(c => c.Id == commentId).ShouldBeFalse();

    }

    [Fact]
    public void ToggleCommentLike_first_time_creates_like_and_liked_true()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        const long blogId = -1;
        EnsureBlogExists(dbContext, -1, -11);

        var author = CreateController(scope, userId: -12);
        (author.AddComment(blogId, new CommentCreateDto { Text = "Komentar za like" }) as OkObjectResult)
            .ShouldNotBeNull();

        var commentId = dbContext.Comments
            .Where(c => c.BlogId == blogId && c.Text == "Komentar za like")
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => c.Id)
            .First();

        var liker = CreateController(scope, userId: -11);

        var res = liker.ToggleCommentLike(blogId, commentId) as OkObjectResult;
        res.ShouldNotBeNull();

        dbContext.CommentLikes.Any(l => l.BlogId == blogId && l.CommentId == commentId && l.UserId == -11)
            .ShouldBeTrue();

        var status = liker.GetCommentLikes(blogId, commentId) as OkObjectResult;
        status.ShouldNotBeNull();

        var json = JsonSerializer.Serialize(status.Value);
        using var doc = JsonDocument.Parse(json);

        doc.RootElement.GetProperty("liked").GetBoolean().ShouldBeTrue();
        doc.RootElement.GetProperty("count").GetInt32().ShouldBeGreaterThan(0);
    }

    [Fact]
    public void ToggleCommentLike_second_time_removes_like_and_liked_false()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        const long blogId = -1;
        EnsureBlogExists(dbContext, -1, -11);

        var author = CreateController(scope, userId: -12);
        (author.AddComment(blogId, new CommentCreateDto { Text = "Komentar za unlike" }) as OkObjectResult)
            .ShouldNotBeNull();

        var commentId = dbContext.Comments
            .Where(c => c.BlogId == blogId && c.Text == "Komentar za unlike")
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => c.Id)
            .First();

        var liker = CreateController(scope, userId: -11);

        (liker.ToggleCommentLike(blogId, commentId) as OkObjectResult).ShouldNotBeNull();
        dbContext.CommentLikes.Any(l => l.BlogId == blogId && l.CommentId == commentId && l.UserId == -11).ShouldBeTrue();

        (liker.ToggleCommentLike(blogId, commentId) as OkObjectResult).ShouldNotBeNull();
        dbContext.CommentLikes.Any(l => l.BlogId == blogId && l.CommentId == commentId && l.UserId == -11).ShouldBeFalse();

        var status = liker.GetCommentLikes(blogId, commentId) as OkObjectResult;
        status.ShouldNotBeNull();

        var json = JsonSerializer.Serialize(status.Value);
        using var doc = JsonDocument.Parse(json);

        doc.RootElement.GetProperty("liked").GetBoolean().ShouldBeFalse();
    }

    [Fact]
    public void ReportComment_creates_report_and_status_true()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        const long blogId = -1;
        EnsureBlogExists(dbContext, -1, -11);

        var author = CreateController(scope, userId: -11);
        (author.AddComment(blogId, new CommentCreateDto { Text = "Komentar za report" }) as OkObjectResult)
            .ShouldNotBeNull();

        var commentId = dbContext.Comments
            .Where(c => c.BlogId == blogId && c.Text == "Komentar za report")
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => c.Id)
            .First();

        var reporter = CreateController(scope, userId: -12);

        var dto = new CommentReportCreateDto
        {
            Reason = 0,
            AdditionalInfo = "Spam"
        };

        (reporter.ReportComment(blogId, commentId, dto) as NoContentResult).ShouldNotBeNull();

        dbContext.CommentReports.Any(r => r.BlogId == blogId && r.CommentId == commentId && r.UserId == -12)
            .ShouldBeTrue();

        var status = reporter.GetReportStatus(blogId, commentId) as OkObjectResult;
        status.ShouldNotBeNull();

        var json = JsonSerializer.Serialize(status.Value);
        using var doc = JsonDocument.Parse(json);

        doc.RootElement.GetProperty("alreadyReported").GetBoolean().ShouldBeTrue();
    }

    [Fact]
    public void ReportComment_duplicate_by_same_user_fails()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        const long blogId = -1;
        EnsureBlogExists(dbContext, -1, -11);

        var author = CreateController(scope, userId: -11);
        (author.AddComment(blogId, new CommentCreateDto { Text = "Komentar za dupli report" }) as OkObjectResult)
            .ShouldNotBeNull();

        var commentId = dbContext.Comments
            .Where(c => c.BlogId == blogId && c.Text == "Komentar za dupli report")
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => c.Id)
            .First();

        var reporter = CreateController(scope, userId: -12);

        var dto = new CommentReportCreateDto { Reason = 0, AdditionalInfo = "x" };

        (reporter.ReportComment(blogId, commentId, dto) as NoContentResult).ShouldNotBeNull();

        Should.Throw<Exception>(() => reporter.ReportComment(blogId, commentId, dto));
    }

    [Fact]
    public void Admin_can_approve_report()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        const long blogId = -1;
        EnsureBlogExists(dbContext, -1, -11);

        var author = CreateController(scope, userId: -11);
        (author.AddComment(blogId, new CommentCreateDto { Text = "Komentar za admin approve" }) as OkObjectResult)
            .ShouldNotBeNull();

        var commentId = dbContext.Comments
            .Where(c => c.BlogId == blogId && c.Text == "Komentar za admin approve")
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => c.Id)
            .First();

        var reporter = CreateController(scope, userId: -12);
        (reporter.ReportComment(blogId, commentId, new CommentReportCreateDto { Reason = 0, AdditionalInfo = "x" }) as NoContentResult)
            .ShouldNotBeNull();

        var reportId = dbContext.CommentReports
            .Where(r => r.BlogId == blogId && r.CommentId == commentId && r.UserId == -12)
            .Select(r => r.Id)
            .First();

        var admin = CreateAdminReportController(scope, adminId: 999);

        (admin.GetOpen() as OkObjectResult).ShouldNotBeNull();

        var approve = admin.Approve(reportId, new CommentReportReviewDto { Note = "ok" }) as NoContentResult;
        approve.ShouldNotBeNull();

        var updated = dbContext.CommentReports.First(r => r.Id == reportId);
        updated.ReportStatus.ShouldBe(AdminReportStatus.APPROVED);
        updated.ReviewerId.ShouldBe(999);
        updated.ReviewedAt.ShouldNotBeNull();
        updated.AdminNote.ShouldBe("ok");
    }

    [Fact]
    public void Admin_can_dismiss_report()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        const long blogId = -1;
        EnsureBlogExists(dbContext, -1, -11);

        var author = CreateController(scope, userId: -11);
        (author.AddComment(blogId, new CommentCreateDto { Text = "Komentar za admin dismiss" }) as OkObjectResult)
            .ShouldNotBeNull();

        var commentId = dbContext.Comments
            .Where(c => c.BlogId == blogId && c.Text == "Komentar za admin dismiss")
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => c.Id)
            .First();

        var reporter = CreateController(scope, userId: -12);
        (reporter.ReportComment(blogId, commentId, new CommentReportCreateDto { Reason = 0, AdditionalInfo = "x" }) as NoContentResult)
            .ShouldNotBeNull();

        var reportId = dbContext.CommentReports
            .Where(r => r.BlogId == blogId && r.CommentId == commentId && r.UserId == -12)
            .Select(r => r.Id)
            .First();

        var admin = CreateAdminReportController(scope, adminId: 999);

        var dismiss = admin.Dismiss(reportId, new CommentReportReviewDto { Note = "nije problem" }) as NoContentResult;
        dismiss.ShouldNotBeNull();

        var updated = dbContext.CommentReports.First(r => r.Id == reportId);
        updated.ReportStatus.ShouldBe(AdminReportStatus.DISMISSED);
        updated.ReviewerId.ShouldBe(999);
        updated.ReviewedAt.ShouldNotBeNull();
        updated.AdminNote.ShouldBe("nije problem");
    }
}
