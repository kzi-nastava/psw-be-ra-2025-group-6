using Explorer.API.Controllers.Tourist;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Execution;
using Explorer.Tours.Infrastructure.Database;
using Explorer.Tours.Infrastructure.Database.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.Payments.Core.Domain;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourExecutionChallengeNotificationTests : BaseToursIntegrationTest
{
    public TourExecutionChallengeNotificationTests(ToursTestFactory factory) : base(factory) { }

    private const long TOURIST_ID = 1;

    [Fact]
    public void CheckProgress_signals_available_challenges_when_near_keypoint()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, TOURIST_ID);
        var toursDbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var paymentsDbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

        // Clean up and create token
        var existingTokens = paymentsDbContext.TourPurchaseTokens
            .Where(t => t.TouristId == TOURIST_ID && t.TourId == -3)
            .ToList();
        paymentsDbContext.TourPurchaseTokens.RemoveRange(existingTokens);
        
        var existingExecutions = toursDbContext.Set<TourExecutionEntity>()
            .Where(e => e.TouristId == TOURIST_ID && e.TourId == -3)
            .ToList();
        toursDbContext.Set<TourExecutionEntity>().RemoveRange(existingExecutions);
        paymentsDbContext.SaveChanges();
        toursDbContext.SaveChanges();

        var token = new TourPurchaseToken(TOURIST_ID, -3, "Tura Pariza", 100);
        paymentsDbContext.TourPurchaseTokens.Add(token);
        paymentsDbContext.SaveChanges();

        // Start execution
        var startDto = new TourExecutionStartDto
        {
            TourId = -3,
            Latitude = 48.8566,
            Longitude = 2.3522
        };

        var startResult = controller.Start(startDto).Result as CreatedAtActionResult;
        var execution = startResult!.Value as TourExecutionStartResultDto;

        // Tourist moves to first keypoint location (where challenges exist)
        var checkDto = new TrackPointDto
        {
            Latitude = execution!.FirstKeyPoint!.Latitude,
            Longitude = execution.FirstKeyPoint.Longitude
        };

        // Act
        var result = controller.CheckProgress(execution.TourExecutionId, checkDto).Result as OkObjectResult;

        // Assert
        result.ShouldNotBeNull();
        var progress = result.Value as ProgressResponseDto;
        progress.ShouldNotBeNull();
        progress.KeyPointCompleted.ShouldBeTrue();
        
        // Should signal that challenges might be available
        progress.HasAvailableChallenges.ShouldBeTrue();
        
        // Note: AvailableChallengeIds will be empty because TourExecutionService
        // doesn't have direct access to Encounters module
        // Frontend should call GET /api/encounters/challenges/keypoint/{keyPointId}
    }

    [Fact]
    public void CheckProgress_does_not_signal_challenges_when_far_from_keypoint()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, TOURIST_ID);
        var paymentsDbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

        var token = new TourPurchaseToken(TOURIST_ID, -3, "Tura Pariza", 100);
        paymentsDbContext.TourPurchaseTokens.Add(token);
        paymentsDbContext.SaveChanges();

        var startDto = new TourExecutionStartDto
        {
            TourId = -3,
            Latitude = 48.8566,
            Longitude = 2.3522
        };

        var startResult = controller.Start(startDto).Result as CreatedAtActionResult;
        var execution = startResult!.Value as TourExecutionStartResultDto;

        // Tourist is far from any keypoint
        var checkDto = new TrackPointDto
        {
            Latitude = 45.0,  // Far from Paris
            Longitude = 19.0
        };

        // Act
        var result = controller.CheckProgress(execution!.TourExecutionId, checkDto).Result as OkObjectResult;

        // Assert
        result.ShouldNotBeNull();
        var progress = result.Value as ProgressResponseDto;
        progress.ShouldNotBeNull();
        progress.KeyPointCompleted.ShouldBeFalse();
        progress.HasAvailableChallenges.ShouldBeFalse();
    }

    private static TourExecutionController CreateController(IServiceScope scope, long touristId)
    {
        return new TourExecutionController(scope.ServiceProvider.GetRequiredService<ITourExecutionService>())
        {
            ControllerContext = BuildContext(touristId.ToString())
        };
    }
}
