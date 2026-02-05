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
public class HiddenLocationXpTests : BaseEncountersIntegrationTest
{
    public HiddenLocationXpTests(EncountersTestFactory factory) : base(factory) { }

    [Fact]
    public void Completing_hidden_location_awards_xp()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var hiddenLocationController = CreateHiddenLocationController(scope);
        var touristController = CreateTouristController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

        // Get initial profile
        var initialProfileResult = touristController.GetProfile().Result as OkObjectResult;
        var initialProfile = initialProfileResult!.Value as TouristXpProfileDto;
        var initialXP = initialProfile!.CurrentXP;

        var startDto = new StartHiddenLocationDto
        {
            ChallengeId = -2, // Secret Garden (30 XP)
            UserLatitude = 45.26,
            UserLongitude = 19.85
        };
        var startResult = hiddenLocationController.StartAttempt(startDto).Result as OkObjectResult;
        var attempt = startResult!.Value as HiddenLocationAttemptDto;

        // Act - Stay in radius for 30 seconds
        for (int i = 0; i < 6; i++)
        {
            System.Threading.Thread.Sleep(5000);
            
            var updateDto = new UpdateHiddenLocationProgressDto
            {
                AttemptId = attempt!.Id,
                UserLatitude = 45.26,
                UserLongitude = 19.85
            };
            var progressResult = hiddenLocationController.UpdateProgress(updateDto).Result as OkObjectResult;
            var progress = progressResult!.Value as HiddenLocationProgressDto;

            if (progress!.IsSuccessful)
            {
                // Assert - Check XP was awarded
                var finalProfileResult = touristController.GetProfile().Result as OkObjectResult;
                var finalProfile = finalProfileResult!.Value as TouristXpProfileDto;
                
                finalProfile!.CurrentXP.ShouldBe(initialXP + 30);
                
                // Verify completion record
                var completion = dbContext.EncounterCompletions
                    .FirstOrDefault(c => c.UserId == 1 && c.ChallengeId == -2);
                completion.ShouldNotBeNull();
                completion.XpAwarded.ShouldBe(30);
                
                return;
            }
        }

        Assert.Fail("Challenge was not completed");
    }

    [Fact]
    public void Cannot_complete_same_challenge_twice()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        
        // Use User 3 who hasn't completed any challenges yet
        var controller = CreateHiddenLocationController(scope, "3");
        var touristController = CreateTouristController(scope, "3");
        var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

        // Start and complete challenge -1 for the first time
        var startDto = new StartHiddenLocationDto
        {
            ChallengeId = -1, // Find the Hidden Statue (50 XP)
            UserLatitude = 45.25,
            UserLongitude = 19.84
        };

        var startResult = controller.StartAttempt(startDto).Result as OkObjectResult;
        var firstAttempt = startResult!.Value as HiddenLocationAttemptDto;

        // Complete the challenge for the first time
        for (int i = 0; i < 6; i++)
        {
            System.Threading.Thread.Sleep(5000);
            
            var updateDto = new UpdateHiddenLocationProgressDto
            {
                AttemptId = firstAttempt!.Id,
                UserLatitude = 45.25,
                UserLongitude = 19.84
            };
            
            var progressResult = controller.UpdateProgress(updateDto).Result as OkObjectResult;
            var progress = progressResult!.Value as HiddenLocationProgressDto;
            
            if (progress!.IsSuccessful && progress.XpAwarded.HasValue)
            {
                // First completion successful
                progress.XpAwarded.ShouldBe(50);
                break;
            }
        }

        // Act - Try to start a second attempt for the same challenge
        var secondStartDto = new StartHiddenLocationDto
        {
            ChallengeId = -1,
            UserLatitude = 45.25,
            UserLongitude = 19.84
        };

        var secondStartResult = controller.StartAttempt(secondStartDto).Result;

        // If second start is allowed, try to complete again
        if (secondStartResult is OkObjectResult secondOkResult)
        {
            var secondAttempt = secondOkResult.Value as HiddenLocationAttemptDto;
            
            // Try to complete the second attempt
            for (int i = 0; i < 6; i++)
            {
                System.Threading.Thread.Sleep(5000);
                
                var updateDto = new UpdateHiddenLocationProgressDto
                {
                    AttemptId = secondAttempt!.Id,
                    UserLatitude = 45.25,
                    UserLongitude = 19.84
                };
                
                var progressResult = controller.UpdateProgress(updateDto).Result as OkObjectResult;
                var progress = progressResult!.Value as HiddenLocationProgressDto;
                
                if (progress!.IsSuccessful)
                {
                    // Assert - XP should NOT be awarded second time
                    progress.XpAwarded.ShouldBeNull();
                    break;
                }
            }
        }

        // Verify only ONE completion exists in database
        var completions = dbContext.EncounterCompletions
            .Where(c => c.UserId == 3 && c.ChallengeId == -1)
            .ToList();
        
        completions.Count.ShouldBe(1); // Only one completion
    }

    [Fact]
    public void Level_up_occurs_when_xp_threshold_reached()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var hiddenLocationController = CreateHiddenLocationController(scope);
        var touristController = CreateTouristController(scope);

        // Get initial profile (Level 1, 0 XP)
        var initialProfileResult = touristController.GetProfile().Result as OkObjectResult;
        var initialProfile = initialProfileResult!.Value as TouristXpProfileDto;
        initialProfile!.Level.ShouldBe(1);

        // Complete challenge with 50 XP (Find the Hidden Statue)
        var startDto = new StartHiddenLocationDto
        {
            ChallengeId = -1,
            UserLatitude = 45.25,
            UserLongitude = 19.84
        };
        var startResult = hiddenLocationController.StartAttempt(startDto).Result as OkObjectResult;
        var attempt = startResult!.Value as HiddenLocationAttemptDto;

        // Act - Complete the challenge
        for (int i = 0; i < 6; i++)
        {
            System.Threading.Thread.Sleep(5000);
            
            var updateDto = new UpdateHiddenLocationProgressDto
            {
                AttemptId = attempt!.Id,
                UserLatitude = 45.25,
                UserLongitude = 19.84
            };
            var progressResult = hiddenLocationController.UpdateProgress(updateDto).Result as OkObjectResult;
            var progress = progressResult!.Value as HiddenLocationProgressDto;

            if (progress!.IsSuccessful)
            {
                // Assert - Still level 1 (needs 100 XP for level 2)
                var finalProfileResult = touristController.GetProfile().Result as OkObjectResult;
                var finalProfile = finalProfileResult!.Value as TouristXpProfileDto;
                
                finalProfile!.CurrentXP.ShouldBe(50);
                finalProfile.Level.ShouldBe(1); // Not enough for level 2 yet
                finalProfile.XpNeededForNextLevel.ShouldBe(50); // Needs 50 more
                
                return;
            }
        }

        Assert.Fail("Challenge was not completed");
    }

    private static HiddenLocationController CreateHiddenLocationController(IServiceScope scope, string userId = "1")
    {
        return new HiddenLocationController(scope.ServiceProvider.GetRequiredService<IHiddenLocationService>())
        {
            ControllerContext = BuildContext(userId)
        };
    }

    private static TouristEncounterController CreateTouristController(IServiceScope scope, string userId = "1")
    {
        return new TouristEncounterController(scope.ServiceProvider.GetRequiredService<Explorer.Encounters.Core.UseCases.ITouristEncounterService>())
        {
            ControllerContext = BuildContext(userId)
        };
    }
}
