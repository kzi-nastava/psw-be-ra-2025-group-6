using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.Core.UseCases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Explorer.API.Controllers.Author.Authoring;

namespace Explorer.Encounters.Tests.Integration;

[Collection("Sequential")]
public class AuthorChallengesControllerTests : BaseEncountersIntegrationTest
{
    public AuthorChallengesControllerTests(EncountersTestFactory factory) : base(factory) { }

    [Fact]
    public void Author_gets_only_their_challenges()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IChallengeService>();
        var controller = new AuthorChallengesController(service);
        
        // Simulate author with ID = 1
        SetupAuthorUser(controller, 1);

        // Act
        var result = controller.GetMyChallenges();

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();
        var challenges = okResult.Value as List<ChallengeDto>;
        challenges.ShouldNotBeNull();
        
        // All challenges should belong to author ID = 1
        challenges.ShouldAllBe(c => c.CreatorId == 1);
        challenges.ShouldAllBe(c => c.KeyPointId.HasValue);
    }

    [Fact]
    public void Author_cannot_update_others_challenge()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IChallengeService>();
        var controller = new AuthorChallengesController(service);
        
        // Author ID = 2 tries to update challenge owned by author ID = 1
        SetupAuthorUser(controller, 2);

        // Assuming challenge with ID -100 belongs to author 1 (from seed data)
        long challengeId = -100; // Challenge that belongs to different author

        // Act
        var result = controller.Update(
            challengeId,
            "Hacked Title",
            "Hacked Description",
            100,
            "Location",
            false,
            50,
            null,
            null,
            null
        ).Result;

        // Assert
        result.Result.ShouldBeOfType<ForbidResult>();
    }

    [Fact]
    public void Author_can_update_their_challenge()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IChallengeService>();
        var controller = new AuthorChallengesController(service);
        
        // Create a challenge for author ID = 1
        SetupAuthorUser(controller, 1);

        // First, create a challenge via TourController (simulated)
        var createDto = new ChallengeDto
        {
            Title = "Original Challenge",
            Description = "Original Description",
            XP = 50,
            Type = "Location",
            IsRequiredForSecret = false,
            ActivationRadiusMeters = 50
        };

        var created = service.CreateForKeyPoint(createDto, -11, 2.2945, 48.8584, 1);

        // Act - Update the challenge
        var result = controller.Update(
            created.Id,
            "Updated Title",
            "Updated Description",
            75,
            "Location",
            true,
            60,
            null,
            null,
            null
        ).Result;

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();
        var updated = okResult.Value as ChallengeDto;
        updated.ShouldNotBeNull();
        updated.Title.ShouldBe("Updated Title");
        updated.Description.ShouldBe("Updated Description");
        updated.XP.ShouldBe(75);
        updated.ActivationRadiusMeters.ShouldBe(60);
        updated.IsRequiredForSecret.ShouldBeTrue();
    }

    [Fact]
    public void Author_can_delete_their_challenge()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IChallengeService>();
        var controller = new AuthorChallengesController(service);
        
        SetupAuthorUser(controller, 1);

        // Create a challenge
        var createDto = new ChallengeDto
        {
            Title = "Challenge to Delete",
            Description = "This will be deleted",
            XP = 50,
            Type = "Misc",
            IsRequiredForSecret = false,
            ActivationRadiusMeters = 50
        };

        var created = service.CreateForKeyPoint(createDto, -11, 2.2945, 48.8584, 1);

        // Act
        var result = controller.Delete(created.Id);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.ShouldNotBeNull();
        
        // Verify it's deleted
        Should.Throw<KeyNotFoundException>(() => service.Get(created.Id));
    }

    [Fact]
    public void Author_cannot_delete_others_challenge()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IChallengeService>();
        var controller = new AuthorChallengesController(service);
        
        // Create challenge owned by author 1
        var createDto = new ChallengeDto
        {
            Title = "Challenge by Author 1",
            Description = "Owned by author 1",
            XP = 50,
            Type = "Location",
            IsRequiredForSecret = false,
            ActivationRadiusMeters = 50
        };

        var created = service.CreateForKeyPoint(createDto, -11, 2.2945, 48.8584, 1);

        // Try to delete as author 2
        SetupAuthorUser(controller, 2);

        // Act
        var result = controller.Delete(created.Id);

        // Assert
        result.ShouldBeOfType<ForbidResult>();
        
        // Verify it's NOT deleted
        var challenge = service.Get(created.Id);
        challenge.ShouldNotBeNull();
    }

    [Fact]
    public void Author_gets_challenges_by_keypoint_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IChallengeService>();
        var controller = new AuthorChallengesController(service);
        
        SetupAuthorUser(controller, 1);

        // Create challenges for keypoint -11
        var dto1 = new ChallengeDto
        {
            Title = "Challenge 1",
            Description = "First challenge",
            XP = 50,
            Type = "Location",
            IsRequiredForSecret = false,
            ActivationRadiusMeters = 50
        };

        var dto2 = new ChallengeDto
        {
            Title = "Challenge 2",
            Description = "Second challenge",
            XP = 75,
            Type = "Misc",
            IsRequiredForSecret = true,
            ActivationRadiusMeters = 60
        };

        service.CreateForKeyPoint(dto1, -11, 2.2945, 48.8584, 1);
        service.CreateForKeyPoint(dto2, -11, 2.2945, 48.8584, 1);

        // Act
        var result = controller.GetByKeyPointId(-11);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();
        var challenges = okResult.Value as List<ChallengeDto>;
        challenges.ShouldNotBeNull();
        challenges.Count.ShouldBeGreaterThanOrEqualTo(2);
        challenges.ShouldAllBe(c => c.KeyPointId == -11);
        challenges.ShouldAllBe(c => c.CreatorId == 1);
    }

    private void SetupAuthorUser(ControllerBase controller, long userId)
    {
        var claims = new List<Claim>
        {
            new Claim("personId", userId.ToString()),
            new Claim(ClaimTypes.Role, "author")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }
}
