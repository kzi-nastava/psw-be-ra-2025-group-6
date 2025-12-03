using Explorer.API.Controllers;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public.Administration;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Infrastructure.Database;
using Explorer.BuildingBlocks.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            existingBlog = new DomainBlog(-11, "Test blog za update", "Opis bloga za update", new List<string>(), BlogStatus.POSTED);

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
        typeof(DomainBlog).GetProperty("Id")?.SetValue(blog, -5);
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
        typeof(DomainBlog).GetProperty("Id")?.SetValue(blog, -6);
        dbContext.Blogs.Add(blog);
        dbContext.SaveChanges();

        var result = controller.DeleteBlog(blog.Id);
        result.ShouldBeOfType<OkObjectResult>();

        var storedBlog = dbContext.Blogs.FirstOrDefault(b => b.Id == blog.Id);
        storedBlog.ShouldBeNull();
    }

    private static BlogController CreateController(IServiceScope scope)
    {
        return new BlogController(scope.ServiceProvider.GetRequiredService<IBlogService>())
        {
            ControllerContext = BuildContext("-11")
        };
    }
}
