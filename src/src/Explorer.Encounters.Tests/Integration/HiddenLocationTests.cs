using Explorer.API.Controllers.Encounters;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Encounters.Tests.Integration;

[Collection("Sequential")]
public class HiddenLocationTests : BaseEncountersIntegrationTest
{
    public HiddenLocationTests(EncountersTestFactory factory) : base(factory) { }

    [Fact]
    public void Starts_hidden_location_attempt_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();
        
        var dto = new StartHiddenLocationDto
        {
            ChallengeId = -1, // "Find the Hidden Statue" at (19.84, 45.25)
            UserLatitude = 45.25,
            UserLongitude = 19.84
        };

        // Act
        var result = controller.StartAttempt(dto).Result as OkObjectResult;

        // Assert
        result.ShouldNotBeNull();
        var attempt = result.Value as HiddenLocationAttemptDto;
        attempt.ShouldNotBeNull();
        attempt.UserId.ShouldBe(1);
        attempt.ChallengeId.ShouldBe(-1);
        attempt.IsSuccessful.ShouldBeFalse(); // Not successful yet
        attempt.SecondsInRadius.ShouldBe(0);
    }

    [Fact]
    public void Start_fails_when_too_far_from_location()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        
        var dto = new StartHiddenLocationDto
        {
            ChallengeId = -1,
            UserLatitude = 45.0, // Far away
            UserLongitude = 19.0
        };

        // Act
        var result = controller.StartAttempt(dto).Result;

        // Assert
        result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Updates_progress_when_in_radius()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Start attempt first
        var startDto = new StartHiddenLocationDto
        {
            ChallengeId = -2, // Secret Garden at (19.85, 45.26)
            UserLatitude = 45.26,
            UserLongitude = 19.85
        };
        var startResult = controller.StartAttempt(startDto).Result as OkObjectResult;
        var attempt = startResult!.Value as HiddenLocationAttemptDto;

        // Wait a bit to simulate time passing
        System.Threading.Thread.Sleep(2000);

        // Act - Update progress while in radius
        var updateDto = new UpdateHiddenLocationProgressDto
        {
            AttemptId = attempt!.Id,
            UserLatitude = 45.26,
            UserLongitude = 19.85
        };
        var result = controller.UpdateProgress(updateDto).Result as OkObjectResult;

        // Assert
        result.ShouldNotBeNull();
        var progress = result.Value as HiddenLocationProgressDto;
        progress.ShouldNotBeNull();
        progress.IsInRadius.ShouldBeTrue();
        progress.SecondsInRadius.ShouldBeGreaterThan(0);
        progress.DistanceToTarget.ShouldBeLessThanOrEqualTo(5.0);
    }

    [Fact]
    public void Resets_timer_when_leaving_radius()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var startDto = new StartHiddenLocationDto
        {
            ChallengeId = -2,
            UserLatitude = 45.26,
            UserLongitude = 19.85
        };
        var startResult = controller.StartAttempt(startDto).Result as OkObjectResult;
        var attempt = startResult!.Value as HiddenLocationAttemptDto;

        System.Threading.Thread.Sleep(2000);

        // First update in radius
        var updateDto1 = new UpdateHiddenLocationProgressDto
        {
            AttemptId = attempt!.Id,
            UserLatitude = 45.26,
            UserLongitude = 19.85
        };
        controller.UpdateProgress(updateDto1);

        System.Threading.Thread.Sleep(2000);

        // Act - Leave radius
        var updateDto2 = new UpdateHiddenLocationProgressDto
        {
            AttemptId = attempt.Id,
            UserLatitude = 45.0, // Far away
            UserLongitude = 19.0
        };
        var result = controller.UpdateProgress(updateDto2).Result as OkObjectResult;

        // Assert
        result.ShouldNotBeNull();
        var progress = result.Value as HiddenLocationProgressDto;
        progress.ShouldNotBeNull();
        progress.IsInRadius.ShouldBeFalse();
        progress.SecondsInRadius.ShouldBe(0); // Timer reset
    }

    [Fact]
    public void Completes_challenge_after_30_seconds()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

        var startDto = new StartHiddenLocationDto
        {
            ChallengeId = -2,
            UserLatitude = 45.26,
            UserLongitude = 19.85
        };
        var startResult = controller.StartAttempt(startDto).Result as OkObjectResult;
        var attempt = startResult!.Value as HiddenLocationAttemptDto;

        // Simulate staying in radius for 30+ seconds
        for (int i = 0; i < 6; i++)
        {
            System.Threading.Thread.Sleep(5000); // 5 seconds each iteration
            
            var updateDto = new UpdateHiddenLocationProgressDto
            {
                AttemptId = attempt!.Id,
                UserLatitude = 45.26,
                UserLongitude = 19.85
            };
            var result = controller.UpdateProgress(updateDto).Result as OkObjectResult;
            var progress = result!.Value as HiddenLocationProgressDto;

            if (progress!.IsSuccessful)
            {
                // Assert
                progress.SecondsInRadius.ShouldBeGreaterThanOrEqualTo(30);
                
                // Verify completion was recorded
                var completion = dbContext.EncounterCompletions
                    .FirstOrDefault(c => c.UserId == 1 && c.ChallengeId == -2);
                completion.ShouldNotBeNull();
                completion.XpAwarded.ShouldBe(30); // XP from challenge -2
                
                return; // Test passed
            }
        }

        Assert.Fail("Challenge did not complete after 30 seconds");
    }

    [Fact]
    public void Retrieves_user_attempt_history()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = controller.GetUserAttempts(-1).Result as OkObjectResult;

        // Assert
        result.ShouldNotBeNull();
        var attempts = result.Value as List<HiddenLocationAttemptDto>;
        attempts.ShouldNotBeNull();
        attempts.Count.ShouldBeGreaterThan(0);
        attempts.Any(a => a.ChallengeId == -1 && a.IsSuccessful).ShouldBeTrue();
    }

    [Fact]
    public void Retrieves_active_attempt()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "2"); // User 2 has active attempt

        // Act
        var result = controller.GetActiveAttempt(-2).Result as OkObjectResult;

        // Assert
        result.ShouldNotBeNull();
        var attempt = result.Value as HiddenLocationAttemptDto;
        attempt.ShouldNotBeNull();
        attempt.ChallengeId.ShouldBe(-2);
        attempt.IsSuccessful.ShouldBeFalse();
        attempt.CompletedAt.ShouldBeNull();
    }

    [Fact]
    public void Start_fails_when_active_attempt_exists()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "2"); // User 2 already has active attempt

        // User 2 already has active attempt for challenge -2
        var dto = new StartHiddenLocationDto
        {
            ChallengeId = -2,
            UserLatitude = 45.26,
            UserLongitude = 19.85
        };

        // Act
        var result = controller.StartAttempt(dto).Result;

        // Assert
        result.ShouldBeOfType<BadRequestObjectResult>();
    }

    private static HiddenLocationController CreateController(IServiceScope scope, string userId = "1")
    {
        return new HiddenLocationController(scope.ServiceProvider.GetRequiredService<IHiddenLocationService>())
        {
            ControllerContext = BuildContext(userId)
        };
    }
}
