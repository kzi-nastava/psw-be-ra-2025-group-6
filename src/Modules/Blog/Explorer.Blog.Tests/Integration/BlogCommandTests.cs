using Explorer.API.Controllers;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public.Administration;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Infrastructure.Database;
using Explorer.BuildingBlocks.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using DomainBlog = Explorer.Blog.Core.Domain.BlogPost;

namespace Explorer.Blog.Tests.Integration.Administration;

[Collection("Sequential")]
public class BlogCommandTests : BaseBlogIntegrationTest
{
    public BlogCommandTests(BlogTestFactory factory) : base(factory) { }

    [Fact]
    public async Task CreatesBlog()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        string title = "Novi test blog";
        string description = "Opis novog test bloga";
        List<IFormFile>? images = null;

        var actionResult = await controller.CreateBlog(title, description, images, BlogStatusDto.DRAFT);
        var okResult = actionResult.Result as OkObjectResult;
        okResult.ShouldNotBeNull();

        var result = okResult.Value as BlogDto;
        result.ShouldNotBeNull();
        result.Id.ShouldBeGreaterThan(0);
        result.Title.ShouldBe(title);
        result.Description.ShouldBe(description);

        var storedBlog = dbContext.Blogs.FirstOrDefault(b => b.Id == result.Id);
        storedBlog.ShouldNotBeNull();
        storedBlog.Title.ShouldBe(title);
        storedBlog.Description.ShouldBe(description);
        storedBlog.UserId.ShouldBe(-11);
        storedBlog.Images.ShouldBeEmpty();
    }

    [Fact]
    public void UpdatesBlog()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        var existingBlog = dbContext.Blogs.FirstOrDefault(b => b.Id == -3);

        if (existingBlog == null)
        {
            existingBlog = new DomainBlog(-11, "Test blog za update", "Opis bloga za update", new List<string>(), BlogStatus.DRAFT);

            typeof(DomainBlog)
                .GetProperty("Id")?
                .SetValue(existingBlog, -3);

            dbContext.Blogs.Add(existingBlog);
            dbContext.SaveChanges();
        }

        var updatedBlog = new BlogDto
        {
            Id = -3,
            Title = "Promenjen naslov bloga",
            Description = "Promenjen opis bloga",
            UserId = existingBlog.UserId,
            CreatedAt = existingBlog.CreatedAt,
            Images = new List<string>()

        };
        existingBlog.Status = BlogStatus.DRAFT;
        dbContext.SaveChanges();
        string title = "Promenjen naslov bloga";
        string description = "Promenjen opis bloga";
        BlogStatusDto status = BlogStatusDto.POSTED;
        List<IFormFile>? images = null;

        var actionResult = controller
            .UpdateBlog(-3, title, description, status, images)
            .GetAwaiter()
            .GetResult();

        var okResult = actionResult.Result as OkObjectResult;
        okResult.ShouldNotBeNull();

        var blogResult = okResult.Value as BlogDto;
        blogResult.ShouldNotBeNull();

        blogResult.Id.ShouldBe(-3);
        blogResult.Title.ShouldBe(title);
        blogResult.Description.ShouldBe(description);

        var storedBlog = dbContext.Blogs.FirstOrDefault(b => b.Id == -3);
        storedBlog.ShouldNotBeNull();
        storedBlog.Title.ShouldBe(title);
        storedBlog.Description.ShouldBe(description);
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        string title = "Nepostojeci blog";
        string description = "Opis";
        BlogStatusDto status = BlogStatusDto.DRAFT;
        List<IFormFile>? images = null;

        Should.Throw<NotFoundException>(() =>
            controller.UpdateBlog(-1000, title, description, status, images)
                     .GetAwaiter()
                     .GetResult()
        );
    }

    [Fact]
    public async Task CreatesBlog_WithStatus()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        string title = "Test blog sa statusom";
        string description = "Opis bloga";
        List<IFormFile>? images = null;
        BlogStatusDto status = BlogStatusDto.POSTED;

        var actionResult = await controller.CreateBlog(title, description, images, status);
        var okResult = actionResult.Result as OkObjectResult;
        okResult.ShouldNotBeNull();

        var result = okResult.Value as BlogDto;
        result.ShouldNotBeNull();
        result.Status.ShouldBe(status);
    }

    [Fact]
    public void ArchiveBlog_SetsStatusToArchived()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        var blog = new DomainBlog(-11, "Blog za arhiviranje", "Opis", new List<string>(), BlogStatus.POSTED);
        dbContext.Blogs.Add(blog);
        dbContext.SaveChanges();

        var result = controller.ArchiveBlog(blog.Id);
        result.ShouldBeOfType<NoContentResult>();

        var storedBlog = dbContext.Blogs.FirstOrDefault(b => b.Id == blog.Id);
        storedBlog.Status.ShouldBe(BlogStatus.ARCHIVED);
    }

    [Fact]
    public void DeleteBlog_RemovesBlog()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        var blog = new DomainBlog(-11, "Blog za brisanje", "Opis", new List<string>(), BlogStatus.POSTED);
        //typeof(DomainBlog).GetProperty("Id")?.SetValue(blog, -116);
        dbContext.Blogs.Add(blog);
        dbContext.SaveChanges();

        var result = controller.DeleteBlog(blog.Id);
        result.ShouldBeOfType<NoContentResult>();

        var storedBlog = dbContext.Blogs.FirstOrDefault(b => b.Id == blog.Id);
        storedBlog.ShouldBeNull();
    }

    [Fact]
    public void VoteOnBlog_UpdatesAggregatedVotes()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        var blog = new DomainBlog(-11, "Test blog", "Opis", new List<string>(), BlogStatus.POSTED);
        dbContext.Blogs.Add(blog);
        dbContext.SaveChanges();

        blog.AddOrUpdateVote(-11, VoteType.Upvote);
        blog.CountUpvotes().ShouldBe(1);
        blog.CountDownvotes().ShouldBe(0);

        blog.AddOrUpdateVote(-11, VoteType.Downvote);
        blog.CountUpvotes().ShouldBe(0);
        blog.CountDownvotes().ShouldBe(1);
    }

    [Fact]
    public void RemoveVote_NonExistingVote_DoesNothing()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        var blog = new DomainBlog(-11, "Blog test", "Opis", new List<string>(), BlogStatus.POSTED);
        dbContext.Blogs.Add(blog);
        dbContext.SaveChanges();

        blog.RemoveVote(999);
        blog.Votes.ShouldBeEmpty();
    }

    [Fact]
    public void CountUpvotesAndDownvotes_ReturnsCorrectValues()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        var blog = new DomainBlog(-11, "Test blog", "Opis", new List<string>(), BlogStatus.POSTED);
        dbContext.Blogs.Add(blog);
        dbContext.SaveChanges();

        blog.AddOrUpdateVote(1, VoteType.Upvote);
        blog.AddOrUpdateVote(2, VoteType.Upvote);
        blog.AddOrUpdateVote(3, VoteType.Downvote);

        blog.CountUpvotes().ShouldBe(2);
        blog.CountDownvotes().ShouldBe(1);
    }

    [Fact]
    public void AddComment_creates_comment_in_db()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        const long blogId = -1;

        var blog = dbContext.Blogs
            .Include(b => b.Comments)
            .FirstOrDefault(b => b.Id == blogId);

        if (blog == null)
        {
            blog = new DomainBlog(-11, "Test blog za komentare", "Opis test bloga", new List<string>(), BlogStatus.POSTED);

            typeof(DomainBlog)
                .GetProperty("Id")?
                .SetValue(blog, blogId);

            dbContext.Blogs.Add(blog);
            dbContext.SaveChanges();

            blog = dbContext.Blogs
                .Include(b => b.Comments)
                .First(b => b.Id == blogId);
        }

        var initialCount = blog.Comments.Count;

        var dto = new CommentCreateDto
        {
            Text = "Novi komentar iz testa"
        };

        var result = controller.AddComment(blogId, dto) as OkObjectResult;
        result.ShouldNotBeNull();

        var commentDto = result.Value as CommentDto;
        commentDto.ShouldNotBeNull();
        commentDto.Text.ShouldBe(dto.Text);
        commentDto.UserId.ShouldBe(-11); 

        var storedBlog = dbContext.Blogs
            .Include(b => b.Comments)
            .First(b => b.Id == blogId);

        storedBlog.Comments.Count.ShouldBe(initialCount + 1);
        storedBlog.Comments.Last().Text.ShouldBe(dto.Text);
        storedBlog.Comments.Last().UserId.ShouldBe(-11);
    }

    [Fact]
    public void EditComment_updates_text_when_author_and_in_time()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        const long blogId = -21;

        var blog = dbContext.Blogs
            .Include(b => b.Comments)
            .FirstOrDefault(b => b.Id == blogId);

        if (blog == null)
        {
            blog = new DomainBlog(-11, $"Test blog {blogId}", "Opis test bloga", new List<string>(), BlogStatus.POSTED);
            typeof(DomainBlog).GetProperty("Id")?.SetValue(blog, blogId);
            dbContext.Blogs.Add(blog);
            dbContext.SaveChanges();

            blog = dbContext.Blogs
                .Include(b => b.Comments)
                .First(b => b.Id == blogId);
        }

        blog.AddComment(-11, "Test author", "Stari tekst");
        dbContext.SaveChanges();

        var before = dbContext.Blogs
            .Include(b => b.Comments)
            .First(b => b.Id == blogId);

        var commentIndex = before.Comments.Count - 1;

        var dto = new CommentCreateDto
        {
            Text = "Izmenjen tekst komentara"
        };

        var actionResult = controller.EditComment(blogId, commentIndex, dto) as OkResult;
        actionResult.ShouldNotBeNull();

        var storedBlog = dbContext.Blogs
            .Include(b => b.Comments)
            .First(b => b.Id == blogId);

        storedBlog.Comments[commentIndex].Text.ShouldBe(dto.Text);
    }

    [Fact]
    public void EditComment_fails_when_different_user()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        const long blogId = -22;

        var blog = dbContext.Blogs
            .Include(b => b.Comments)
            .FirstOrDefault(b => b.Id == blogId);

        if (blog == null)
        {
            blog = new DomainBlog(-11, $"Test blog {blogId}", "Opis test bloga", new List<string>(), BlogStatus.POSTED);
            typeof(DomainBlog).GetProperty("Id")?.SetValue(blog, blogId);
            dbContext.Blogs.Add(blog);
            dbContext.SaveChanges();

            blog = dbContext.Blogs
                .Include(b => b.Comments)
                .First(b => b.Id == blogId);
        }

        blog.AddComment(-12, "Other user", "Tekst komentara");
        dbContext.SaveChanges();

        var dto = new CommentCreateDto
        {
            Text = "Pokusaj izmene"
        };

        Should.Throw<InvalidOperationException>(() =>
            controller.EditComment(blogId, 0, dto));
    }

    [Fact]
    public void DeleteComment_removes_comment()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        const long blogId = -23;

        var blog = dbContext.Blogs
            .Include(b => b.Comments)
            .FirstOrDefault(b => b.Id == blogId);

        if (blog == null)
        {
            blog = new DomainBlog(-11, $"Test blog {blogId}", "Opis test bloga", new List<string>(), BlogStatus.POSTED);
            typeof(DomainBlog).GetProperty("Id")?.SetValue(blog, blogId);
            dbContext.Blogs.Add(blog);
            dbContext.SaveChanges();
        }

        controller.AddComment(blogId, new CommentCreateDto
        {
            Text = "Neki drugi komentar"
        });
        dbContext.SaveChanges();

        controller.AddComment(blogId, new CommentCreateDto
        {
            Text = "Tekst za brisanje"
        });
        dbContext.SaveChanges();

        var before = dbContext.Blogs
            .Include(b => b.Comments)
            .First(b => b.Id == blogId);

        before.Comments.Count.ShouldBeGreaterThan(1); 

        var commentIndex = before.Comments
            .Select((c, index) => new { Comment = c, Index = index })
            .First(x => x.Comment.Text == "Tekst za brisanje")
            .Index;

        var result = controller.DeleteComment(blogId, commentIndex) as NoContentResult;
        result.ShouldNotBeNull();

        var after = dbContext.Blogs
            .Include(b => b.Comments)
            .First(b => b.Id == blogId);

        after.Comments.Count.ShouldBeGreaterThan(0);
        after.Comments.Any(c => c.Text == "Tekst za brisanje").ShouldBeFalse();
    }

    [Fact]
    public void RemoveVote_ExistingVote_RemovesVote()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        var blog = new DomainBlog(-11, "Blog za test remove vote", "Opis", new List<string>(), BlogStatus.POSTED);
        dbContext.Blogs.Add(blog);
        dbContext.SaveChanges();

        blog.AddOrUpdateVote(1, VoteType.Upvote);
        blog.Votes.Count.ShouldBe(1);

        blog.RemoveVote(1);

        blog.Votes.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(-20, 0, BlogQualityStatus.Closed)]
    [InlineData(600, 40, BlogQualityStatus.Famous)]
    [InlineData(200, 5, BlogQualityStatus.Active)]
    [InlineData(50, 15, BlogQualityStatus.Active)]
    [InlineData(50, 5, BlogQualityStatus.None)]
    public void UpdateQualityStatus_SetsCorrectStatus(int score, int commentCount, BlogQualityStatus expected)
    {
        var blog = new DomainBlog(-11, "Blog za test quality", "Opis", new List<string>(), BlogStatus.POSTED);

        blog.UpdateQualityStatus(score, commentCount);

        blog.QualityStatus.ShouldBe(expected);
    }

    [Fact]
    public void RecalculateQualityStatus_CalculatesCorrectly()
    {
        var blog = new DomainBlog(-11, "Blog za test recalc", "Opis", new List<string>(), BlogStatus.POSTED);

        for (int i = 0; i < 5; i++)
            blog.AddOrUpdateVote(-11, VoteType.Upvote); 
        for (int i = 0; i < 3; i++)
            blog.AddOrUpdateVote(-11, VoteType.Downvote); 

        for (int i = 0; i < 12; i++)
            blog.AddComment(-11, "Test user", $"Komentar {i}");

        blog.RecalculateQualityStatus();

        blog.QualityStatus.ShouldBe(BlogQualityStatus.Active);
    }

    [Fact]
    public void RecalculateQualityStatus_SetsClosed_WhenScoreTooLow()
    {
        var blog = new DomainBlog(-11, "Blog za test recalc closed", "Opis", new List<string>(), BlogStatus.POSTED);

        for (int i = 0; i < 15; i++)
            blog.AddOrUpdateVote(i + 1, VoteType.Downvote);

        blog.RecalculateQualityStatus();

        blog.QualityStatus.ShouldBe(BlogQualityStatus.Closed);
    }

    [Fact]
    public void RecalculateQualityStatus_SetsFamous_WhenHighScoreAndManyComments()
    {
        var blog = new DomainBlog(-11, "Blog za test recalc famous", "Opis", new List<string>(), BlogStatus.POSTED);

        for (int i = 1; i <= 600; i++)
            blog.AddOrUpdateVote(i, VoteType.Upvote);

        for (int i = 1; i <= 40; i++)
            blog.AddComment(i, $"User{i}", $"Komentar {i}");

        blog.RecalculateQualityStatus();

        blog.QualityStatus.ShouldBe(BlogQualityStatus.Famous);
    }

    private static BlogController CreateController(IServiceScope scope)
    {
        var blogService = scope.ServiceProvider.GetRequiredService<IBlogService>();

        return new BlogController(blogService)
        {
            ControllerContext = BuildContext("-11")
        };
    }
}
