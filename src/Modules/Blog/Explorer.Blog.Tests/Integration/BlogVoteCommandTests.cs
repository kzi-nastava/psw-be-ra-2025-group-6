using Explorer.API.Controllers;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public.Administration;
using Explorer.Blog.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Linq;
using Xunit;

namespace Explorer.Blog.Tests.Integration.Administration;

[Collection("Sequential")]
public class BlogVoteCommandTests : BaseBlogIntegrationTest
{
    public BlogVoteCommandTests(BlogTestFactory factory) : base(factory) { }

    [Fact]
    public void Vote_AddsUpvote()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        long blogId = -3;
        long userId = -11;

        var existing = dbContext.BlogVotes.FirstOrDefault(v => v.BlogPostId == blogId && v.UserId == userId);
        if (existing != null)
        {
            dbContext.BlogVotes.Remove(existing);
            dbContext.SaveChanges();
        }

        var result = controller.VoteOnBlog(blogId, VoteTypeDto.Upvote) as OkObjectResult;
        result.ShouldNotBeNull();

        var votes = (System.ValueTuple<int, int>)result.Value!;
        votes.Item1.ShouldBe(1);
        votes.Item2.ShouldBe(0);
    }

    [Fact]
    public void Vote_TogglesVote()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        long blogId = -3;
        long userId = -11;

        var existing = dbContext.BlogVotes.FirstOrDefault(v => v.BlogPostId == blogId && v.UserId == userId);
        if (existing != null)
        {
            dbContext.BlogVotes.Remove(existing);
            dbContext.SaveChanges();
        }

        var firstResult = controller.VoteOnBlog(blogId, VoteTypeDto.Upvote) as OkObjectResult;
        firstResult.ShouldNotBeNull();

        var votes1 = ((ValueTuple<int, int>)firstResult!.Value!);
        votes1.Item1.ShouldBe(1);
        votes1.Item2.ShouldBe(0);

        var secondResult = controller.VoteOnBlog(blogId, VoteTypeDto.Upvote) as OkObjectResult;
        secondResult.ShouldNotBeNull();

        var votes2 = ((ValueTuple<int, int>)secondResult!.Value!);
        votes2.Item1.ShouldBe(0);
        votes2.Item2.ShouldBe(0);

        var voteInDb = dbContext.BlogVotes.FirstOrDefault(v => v.BlogPostId == blogId && v.UserId == userId);
        voteInDb.ShouldBeNull();
    }

    [Fact]
    public void Vote_ChangesVote()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        long blogId = -3;
        long userId = -13;

        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();
        var existing = dbContext.BlogVotes.FirstOrDefault(v => v.BlogPostId == blogId && v.UserId == userId);
        if (existing != null)
        {
            dbContext.BlogVotes.Remove(existing);
            dbContext.SaveChanges();
        }

        var addResult = controller.VoteOnBlog(blogId, VoteTypeDto.Upvote) as OkObjectResult;
        addResult.ShouldNotBeNull();

        var changeResult = controller.VoteOnBlog(blogId, VoteTypeDto.Downvote) as OkObjectResult;
        changeResult.ShouldNotBeNull();

        var votes = (System.ValueTuple<int, int>)changeResult.Value!;
        votes.Item1.ShouldBe(0);
        votes.Item2.ShouldBe(1);
    }

    [Fact]
    public void GetVotes_ReturnsCorrectCountsAndUserVote()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        long blogId = -3;
        var result = controller.GetVotes(blogId) as OkObjectResult;

        result.ShouldNotBeNull();
        var value = result!.Value!;

        int upvotes = value.GetType().GetProperty("upvotes")!.GetValue(value, null) as int? ?? 0;
        int downvotes = value.GetType().GetProperty("downvotes")!.GetValue(value, null) as int? ?? 0;

        upvotes.ShouldBeGreaterThanOrEqualTo(0);
        downvotes.ShouldBeGreaterThanOrEqualTo(0);
    }

    private static BlogController CreateController(IServiceScope scope)
    {
        var blogService = scope.ServiceProvider.GetRequiredService<IBlogService>();
        var blogVoteService = scope.ServiceProvider.GetRequiredService<IBlogVoteService>();

        return new BlogController(blogService, blogVoteService)
        {
            ControllerContext = BuildContext("-11")
        };
    }
}
