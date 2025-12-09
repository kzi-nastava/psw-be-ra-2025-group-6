using Explorer.API.Controllers;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public.Administration;
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

        var actionResult = await controller.CreateBlog(title, description, images);
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
            existingBlog = new DomainBlog(-11, "Test blog za update", "Opis bloga za update", new List<string>());

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

        var result = ((ObjectResult)controller.UpdateBlog(existingBlog.Id, updatedBlog).Result)?.Value as BlogDto;

        result.ShouldNotBeNull();
        ((long)result.Id).ShouldBe(existingBlog.Id);
        result.Title.ShouldBe(updatedBlog.Title);
        result.Description.ShouldBe(updatedBlog.Description);

        var storedBlog = dbContext.Blogs.FirstOrDefault(b => b.Id == existingBlog.Id);
        storedBlog.ShouldNotBeNull();
        storedBlog.Title.ShouldBe(updatedBlog.Title);
        storedBlog.Description.ShouldBe(updatedBlog.Description);
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var updatedBlog = new BlogDto
        {
            Id = -1000,
            Title = "Nepostojeci blog",
            Description = "Opis",
            UserId = -11,
            CreatedAt = DateTime.UtcNow,
            Images = new List<string>()
        };

        Should.Throw<NotFoundException>(() => controller.UpdateBlog(-1000, updatedBlog));
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
            blog = new DomainBlog(-11, "Test blog za komentare", "Opis test bloga", new List<string>());

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
            blog = new DomainBlog(-11, $"Test blog {blogId}", "Opis test bloga", new List<string>());
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
            blog = new DomainBlog(-11, $"Test blog {blogId}", "Opis test bloga", new List<string>());
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
            blog = new DomainBlog(-11, $"Test blog {blogId}", "Opis test bloga", new List<string>());
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

    private static BlogController CreateController(IServiceScope scope)
    {
        return new BlogController(scope.ServiceProvider.GetRequiredService<IBlogService>())
        {
            ControllerContext = BuildContext("-11")
        };
    }
}
