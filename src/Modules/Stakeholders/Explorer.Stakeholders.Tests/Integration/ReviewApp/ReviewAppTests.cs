using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Explorer.API.Controllers;
using Explorer.Stakeholders.API.Dtos.ReviewAppDtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Stakeholders.Tests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

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
        var controller = CreateController(scope, -11);
        var dto = new CreateReviewAppDto
        {
            Rating = 5,
            Comment = "Odlična aplikacija"
        };

        var result = controller.Create(dto);
        var created = (ReviewAppDto)((OkObjectResult)result.Result!).Value!;

        created.UserId.ShouldBe(-11);
        created.Rating.ShouldBe(5);
        created.Comment.ShouldBe("Odlična aplikacija");
        created.CreatedAt.ShouldBeInRange(DateTime.UtcNow.AddMinutes(-1), DateTime.UtcNow.AddMinutes(1));
        created.UpdatedAt.ShouldBeNull();

        dbContext.ChangeTracker.Clear();
        var stored = dbContext.ReviewApps.Single(r => r.Id == created.Id);
        stored.UserId.ShouldBe(-11);
        stored.Rating.ShouldBe(5);
        stored.Comment.ShouldBe("Odlična aplikacija");
    }

    [Fact]
    public void Tourist_can_create_review_without_comment()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateController(scope, -21);
        var dto = new CreateReviewAppDto
        {
            Rating = 4,
            Comment = null
        };

        var result = controller.Create(dto);
        var created = (ReviewAppDto)((OkObjectResult)result.Result!).Value!;

        created.UserId.ShouldBe(-21);
        created.Rating.ShouldBe(4);
        created.Comment.ShouldBeNull();
        created.CreatedAt.ShouldBeInRange(DateTime.UtcNow.AddMinutes(-1), DateTime.UtcNow.AddMinutes(1));

        dbContext.ChangeTracker.Clear();
        var stored = dbContext.ReviewApps.Single(r => r.Id == created.Id);
        stored.Comment.ShouldBeNull();
    }

    [Fact]
    public void Admin_can_get_all_reviews()
    {
        using var scope = Factory.Services.CreateScope();
        var controllerForTourist = CreateController(scope, -21);
        controllerForTourist.Create(new CreateReviewAppDto { Rating = 5, Comment = "Super" });

        var controller = CreateController(scope, -1);

        var result = controller.GetAll();
        var allReviews = (List<ReviewAppDto>)((OkObjectResult)result.Result!).Value!;

        allReviews.Count.ShouldBeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public void User_can_get_only_their_reviews()
    {
        using var scope = Factory.Services.CreateScope();
        var controllerTourist1 = CreateController(scope, -21);
        controllerTourist1.Create(new CreateReviewAppDto { Rating = 5, Comment = "T1" });

        var controllerTourist2 = CreateController(scope, -22);
        controllerTourist2.Create(new CreateReviewAppDto { Rating = 3, Comment = "T2" });

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
        var controller = CreateController(scope, -21);
        var created = (ReviewAppDto)((OkObjectResult)controller.Create(new CreateReviewAppDto
        {
            Rating = 3,
            Comment = "Initial"
        }).Result!).Value!;

        var updateDto = new UpdateReviewAppDto
        {
            Rating = 4,
            Comment = "Updated"
        };

        var result = controller.Update(created.Id, updateDto);
        var updated = (ReviewAppDto)((OkObjectResult)result.Result!).Value!;

        updated.Rating.ShouldBe(4);
        updated.Comment.ShouldBe("Updated");
        updated.UpdatedAt.ShouldNotBeNull();

        dbContext.ChangeTracker.Clear();
        var stored = dbContext.ReviewApps.Single(r => r.Id == created.Id);
        stored.Rating.ShouldBe(4);
        stored.Comment.ShouldBe("Updated");
        stored.UpdatedAt.ShouldNotBeNull();
    }

    [Fact]
    public void Non_owner_cannot_update_review()
    {
        using var scope = Factory.Services.CreateScope();
        var controllerOwner = CreateController(scope, -21);
        var created = (ReviewAppDto)((OkObjectResult)controllerOwner.Create(new CreateReviewAppDto
        {
            Rating = 2,
            Comment = "Owner review"
        }).Result!).Value!;

        var controllerOther = CreateController(scope, -22);
        var updateDto = new UpdateReviewAppDto
        {
            Rating = 5,
            Comment = "Hacked"
        };

        Should.Throw<UnauthorizedAccessException>(() =>
        {
            controllerOther.Update(created.Id, updateDto);
        });
    }

    [Fact]
    public void Owner_can_delete_review()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateController(scope, -21);
        var created = (ReviewAppDto)((OkObjectResult)controller.Create(new CreateReviewAppDto
        {
            Rating = 5,
            Comment = "To delete"
        }).Result!).Value!;

        controller.Delete(created.Id);

        dbContext.ChangeTracker.Clear();
        dbContext.ReviewApps.Any(r => r.Id == created.Id).ShouldBeFalse();
    }

    [Fact]
    public void Non_owner_cannot_delete_review()
    {
        using var scope = Factory.Services.CreateScope();
        var controllerOwner = CreateController(scope, -21);
        var created = (ReviewAppDto)((OkObjectResult)controllerOwner.Create(new CreateReviewAppDto
        {
            Rating = 1,
            Comment = "Owner will keep"
        }).Result!).Value!;

        var controllerOther = CreateController(scope, -22);

        Should.Throw<UnauthorizedAccessException>(() =>
        {
            controllerOther.Delete(created.Id);
        });
    }

    [Fact]
    public void Creating_review_with_invalid_rating_throws()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -21);

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
