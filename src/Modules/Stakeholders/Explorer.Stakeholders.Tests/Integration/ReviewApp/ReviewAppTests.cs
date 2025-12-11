using Explorer.API.Controllers;
using Explorer.Stakeholders.API.Dtos.ReviewAppDtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;

namespace Explorer.Stakeholders.Tests.Integration.ReviewApp;

[Collection("Sequential")]
public class ReviewAppTests : BaseStakeholdersIntegrationTest
{
    public ReviewAppTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Author_can_create_review_with_comment()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateController(scope, -13);
        var dto = new CreateReviewAppDto
        {
            Rating = 5,
            Comment = "Odlična aplikacija"
        };

        var result = controller.Create(dto);
        var created = (ReviewAppDto)((OkObjectResult)result.Result!).Value!;

        created.UserId.ShouldBe(-13);
        created.Rating.ShouldBe(5);
        created.Comment.ShouldBe("Odlična aplikacija");
        created.CreatedAt.ShouldBeInRange(DateTime.UtcNow.AddMinutes(-1), DateTime.UtcNow.AddMinutes(1));
        created.UpdatedAt.ShouldBeNull();

        dbContext.ChangeTracker.Clear();
        var stored = dbContext.ReviewApps.Single(r => r.Id == created.Id);
        stored.UserId.ShouldBe(-13);
        stored.Rating.ShouldBe(5);
        stored.Comment.ShouldBe("Odlična aplikacija");
    }

    [Fact]
    public void Tourist_can_create_review_without_comment()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateController(scope, -23);
        var dto = new CreateReviewAppDto
        {
            Rating = 4,
            Comment = null
        };

        var result = controller.Create(dto);
        var created = (ReviewAppDto)((OkObjectResult)result.Result!).Value!;

        created.UserId.ShouldBe(-23);
        created.Rating.ShouldBe(4);
        created.Comment.ShouldBeNull();
        created.CreatedAt.ShouldBeInRange(DateTime.UtcNow.AddMinutes(-1), DateTime.UtcNow.AddMinutes(1));

        dbContext.ChangeTracker.Clear();
        var stored = dbContext.ReviewApps.Single(r => r.Id == created.Id);
        stored.Comment.ShouldBeNull();
    }

    [Fact]
    public void User_cannot_create_more_than_one_review()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -12);

        controller.Create(new CreateReviewAppDto { Rating = 5, Comment = "First" });

        var second = new CreateReviewAppDto { Rating = 4, Comment = "Second" };
        Should.Throw<InvalidOperationException>(() => controller.Create(second));
    }

    [Fact]
    public void Admin_can_get_all_reviews()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -1);

        var result = controller.GetAll();
        var allReviews = (List<ReviewAppDto>)((OkObjectResult)result.Result!).Value!;

        allReviews.Count.ShouldBeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public void User_can_get_only_their_reviews()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -21);

        var result = controller.GetByUser();
        var reviews = (List<ReviewAppDto>)((OkObjectResult)result.Result!).Value!;

        reviews.ShouldNotBeEmpty();
        reviews.All(r => r.UserId == -21).ShouldBeTrue();
    }

    [Fact]
    public void Owner_can_update_review_and_sets_updatedAt()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateController(scope, -11);
        var createdId = -101L;

        var updateDto = new UpdateReviewAppDto
        {
            Rating = 4,
            Comment = "Updated"
        };

        var result = controller.Update(createdId, updateDto);
        var updated = (ReviewAppDto)((OkObjectResult)result.Result!).Value!;

        updated.Rating.ShouldBe(4);
        updated.Comment.ShouldBe("Updated");
        updated.UpdatedAt.ShouldNotBeNull();

        dbContext.ChangeTracker.Clear();
        var stored = dbContext.ReviewApps.Single(r => r.Id == createdId);
        stored.Rating.ShouldBe(4);
        stored.Comment.ShouldBe("Updated");
        stored.UpdatedAt.ShouldNotBeNull();
    }

    [Fact]
    public void Non_owner_cannot_update_review()
    {
        using var scope = Factory.Services.CreateScope();
        var controllerOther = CreateController(scope, -21);
        var updateDto = new UpdateReviewAppDto
        {
            Rating = 5,
            Comment = "Hacked"
        };

        Should.Throw<UnauthorizedAccessException>(() =>
        {
            controllerOther.Update(-101, updateDto);
        });
    }

    [Fact]
    public void Owner_can_delete_review()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateController(scope, -22);

        controller.Delete(-103);

        dbContext.ChangeTracker.Clear();
        dbContext.ReviewApps.Any(r => r.Id == -103).ShouldBeFalse();
    }

    [Fact]
    public void Non_owner_cannot_delete_review()
    {
        using var scope = Factory.Services.CreateScope();
        var controllerOther = CreateController(scope, -22);

        Should.Throw<UnauthorizedAccessException>(() =>
        {
            controllerOther.Delete(-102);
        });
    }

    [Fact]
    public void Creating_review_with_invalid_rating_throws()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -31);

        var dtoLow = new CreateReviewAppDto { Rating = 0, Comment = "Too low" };
        Should.Throw<ArgumentException>(() => controller.Create(dtoLow));

        var dtoHigh = new CreateReviewAppDto { Rating = 6, Comment = "Too high" };
        Should.Throw<ArgumentException>(() => controller.Create(dtoHigh));
    }

    private static ReviewAppController CreateController(IServiceScope scope, long userId)
    {
        var controller = new ReviewAppController(scope.ServiceProvider.GetRequiredService<IReviewAppService>());
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("id", userId.ToString())
        }, "TestAuth"));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = user
            }
        };

        return controller;
    }
}
