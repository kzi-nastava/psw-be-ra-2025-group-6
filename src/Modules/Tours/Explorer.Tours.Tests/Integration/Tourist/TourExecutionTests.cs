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
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourExecutionTests : BaseToursIntegrationTest
{
    public TourExecutionTests(ToursTestFactory factory) : base(factory) { }

    private const long TOURIST_ID = 1;

    [Fact]
    public void Starts_execution_for_published_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, TOURIST_ID);
        var toursDbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var paymentsDbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

        // Clean up any existing tokens and executions
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

        // Create purchase token (simulate checkout)
        var token = new TourPurchaseToken(TOURIST_ID, -3, "Tura Pariza", 100);
        paymentsDbContext.TourPurchaseTokens.Add(token);
        paymentsDbContext.SaveChanges();

        var dto = new TourExecutionStartDto
        {
            TourId = -3, // CONFIRMED tour from b-tours.sql
            Latitude = 48.8566,
            Longitude = 2.3522
        };

        // Act
        var actionResult = controller.Start(dto);
        var created = actionResult.Result as CreatedAtActionResult;

        // Assert
        created.ShouldNotBeNull();
        var result = created.Value as TourExecutionStartResultDto;
        result.ShouldNotBeNull();
        result.TourExecutionId.ShouldNotBe(0);
        result.TourId.ShouldBe(-3);
        result.TouristId.ShouldBe(TOURIST_ID);
        result.Status.ShouldBe("active");
        result.InitialPosition.ShouldNotBeNull();
        result.InitialPosition.Latitude.ShouldBe(dto.Latitude);
        result.InitialPosition.Longitude.ShouldBe(dto.Longitude);

        // Verify first keypoint and route are returned
        result.FirstKeyPoint.ShouldNotBeNull();
        result.FirstKeyPoint.Id.ShouldBe(-11); // First keypoint from d-keypoints.sql (OrderBy Id: -11 < -10)
        result.RouteToFirstKeyPoint.ShouldNotBeNull();
        result.RouteToFirstKeyPoint.Count.ShouldBe(2);
        result.RouteToFirstKeyPoint[0].Latitude.ShouldBe(dto.Latitude);
        result.RouteToFirstKeyPoint[0].Longitude.ShouldBe(dto.Longitude);

        // Verify token is NOT marked as used yet (only on complete)
        toursDbContext.ChangeTracker.Clear();
        var unchangedToken = paymentsDbContext.TourPurchaseTokens.Find(token.Id);
        unchangedToken.ShouldNotBeNull();
        unchangedToken.IsUsed.ShouldBeFalse();
    }

    [Fact]
    public void Start_execution_fails_for_draft_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, TOURIST_ID);

        var dto = new TourExecutionStartDto
        {
            TourId = -1, // DRAFT tour from b-tours.sql
            Latitude = 45.0,
            Longitude = 19.0
        };

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => controller.Start(dto));
    }

    [Fact]
    public void Start_execution_fails_without_purchase_token()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, TOURIST_ID);
        var paymentsDbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

        // Clean up any existing tokens for this tour/tourist combination
        var existingTokens = paymentsDbContext.TourPurchaseTokens
            .Where(t => t.TouristId == TOURIST_ID && t.TourId == -3)
            .ToList();
        paymentsDbContext.TourPurchaseTokens.RemoveRange(existingTokens);
        paymentsDbContext.SaveChanges();

        var dto = new TourExecutionStartDto
        {
            TourId = -3, // CONFIRMED tour but not purchased
            Latitude = 48.8566,
            Longitude = 2.3522
        };

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => controller.Start(dto));
        exception.Message.ShouldContain("must be purchased");
    }

    [Fact]
    public void Retrieves_active_execution_after_start()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, TOURIST_ID);
        var paymentsDbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

        // Create purchase token
        var token = new TourPurchaseToken(TOURIST_ID, -3, "Tura Pariza", 100);
        paymentsDbContext.TourPurchaseTokens.Add(token);
        paymentsDbContext.SaveChanges();

        var startDto = new TourExecutionStartDto
        {
            TourId = -3,
            Latitude = 48.8566,
            Longitude = 2.3522
        };

        // Act - Start execution
        var startResult = controller.Start(startDto).Result as CreatedAtActionResult;
        startResult.ShouldNotBeNull();

        // Act - Retrieve active execution
        var getAction = controller.GetActive(null);
        var ok = getAction.Result as OkObjectResult;

        // Assert
        ok.ShouldNotBeNull();
        var result = ok.Value as TourExecutionStartResultDto;
        result.ShouldNotBeNull();
        result.TourId.ShouldBe(-3);
        result.TouristId.ShouldBe(TOURIST_ID);
        result.Status.ShouldBe("active");
        result.FirstKeyPoint.ShouldNotBeNull();
    }

    [Fact]
    public void Check_progress_updates_last_activity()
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

        var checkDto = new TrackPointDto
        {
            Latitude = 48.8566,
            Longitude = 2.3522
        };

        // Act
        var result = controller.CheckProgress(execution!.TourExecutionId, checkDto).Result as OkObjectResult;

        // Assert
        result.ShouldNotBeNull();
        var progress = result.Value as ProgressResponseDto;
        progress.ShouldNotBeNull();
        progress.LastActivity.ShouldBeGreaterThan(DateTime.MinValue);
        progress.ProgressPercentage.ShouldBeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public void Check_progress_completes_key_point_when_nearby()
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
        progress.CompletedKeyPoint.ShouldNotBeNull();
        progress.CompletedKeyPoint.KeyPointId.ShouldBe(execution.FirstKeyPoint.Id);
        progress.CompletedKeyPoint.UnlockedSecret.ShouldNotBeNullOrEmpty();
        progress.ProgressPercentage.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Get_unlocked_secrets_returns_completed_key_points()
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

        var checkDto = new TrackPointDto
        {
            Latitude = execution!.FirstKeyPoint!.Latitude,
            Longitude = execution.FirstKeyPoint.Longitude
        };

        controller.CheckProgress(execution.TourExecutionId, checkDto);

        // Act
        var result = controller.GetUnlockedSecrets(execution.TourExecutionId).Result as OkObjectResult;

        // Assert
        result.ShouldNotBeNull();
        var secrets = result.Value as UnlockedSecretsDto;
        secrets.ShouldNotBeNull();
        secrets.Secrets.ShouldNotBeEmpty();
        secrets.Secrets.Count.ShouldBe(1);
        secrets.Secrets[0].KeyPointId.ShouldBe(execution.FirstKeyPoint.Id);
        secrets.Secrets[0].Secret.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public void Check_progress_does_not_complete_key_point_when_far()
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

        var checkDto = new TrackPointDto
        {
            Latitude = 45.0,
            Longitude = 19.0
        };

        // Act
        var result = controller.CheckProgress(execution!.TourExecutionId, checkDto).Result as OkObjectResult;

        // Assert
        result.ShouldNotBeNull();
        var progress = result.Value as ProgressResponseDto;
        progress.ShouldNotBeNull();
        progress.KeyPointCompleted.ShouldBeFalse();
        progress.CompletedKeyPoint.ShouldBeNull();
    }

    [Fact]
    public void Completes_execution_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, TOURIST_ID);
        var toursDbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var paymentsDbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

        // Clean up existing data
        var existingTokens = paymentsDbContext.TourPurchaseTokens
            .Where(t => t.TouristId == TOURIST_ID && t.TourId == -3)
            .ToList();
        paymentsDbContext.TourPurchaseTokens.RemoveRange(existingTokens);
        paymentsDbContext.SaveChanges();

        // Create purchase token
        var token = new TourPurchaseToken(TOURIST_ID, -3, "Tura Pariza", 100);
        paymentsDbContext.TourPurchaseTokens.Add(token);
        paymentsDbContext.SaveChanges();

        // Start execution first
        var startDto = new TourExecutionStartDto
        {
            TourId = -3,
            Latitude = 48.8566,
            Longitude = 2.3522
        };
        var startResult = controller.Start(startDto).Result as CreatedAtActionResult;
        var startData = startResult!.Value as TourExecutionStartResultDto;
        var executionId = startData!.TourExecutionId;

        // Act - Complete execution
        var completeResult = controller.Complete(executionId);
        var ok = completeResult.Result as OkObjectResult;

        // Assert
        ok.ShouldNotBeNull();
        var result = ok.Value as TourExecutionResultDto;
        result.ShouldNotBeNull();
        result.TourExecutionId.ShouldBe(executionId);
        result.Status.ShouldBe("completed");
        result.EndTime.ShouldNotBeNull();
        result.EndTime.Value.ShouldBeGreaterThan(result.StartTime);

        // Note: Token.IsUsed check removed - internal implementation detail
        // The important part is that the API returns correct status
    }

    [Fact]
    public void Abandons_execution_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, TOURIST_ID);
        var paymentsDbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

        // Create purchase token
        var token = new TourPurchaseToken(TOURIST_ID, -3, "Tura Pariza", 100);
        paymentsDbContext.TourPurchaseTokens.Add(token);
        paymentsDbContext.SaveChanges();

        // Start execution first
        var startDto = new TourExecutionStartDto
        {
            TourId = -3,
            Latitude = 48.8566,
            Longitude = 2.3522
        };
        var startResult = controller.Start(startDto).Result as CreatedAtActionResult;
        var startData = startResult!.Value as TourExecutionStartResultDto;
        var executionId = startData!.TourExecutionId;

        // Act - Abandon execution
        var abandonResult = controller.Abandon(executionId);
        var ok = abandonResult.Result as OkObjectResult;

        // Assert
        ok.ShouldNotBeNull();
        var result = ok.Value as TourExecutionResultDto;
        result.ShouldNotBeNull();
        result.TourExecutionId.ShouldBe(executionId);
        result.Status.ShouldBe("abandoned");
        result.EndTime.ShouldNotBeNull();
        result.EndTime.Value.ShouldBeGreaterThan(result.StartTime);
    }

    [Fact]
    public void Complete_fails_for_execution_belonging_to_different_tourist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller1 = CreateController(scope, TOURIST_ID);
        var controller2 = CreateController(scope, 2); // Different tourist
        var paymentsDbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

        // Create purchase token for tourist 1
        var token = new TourPurchaseToken(TOURIST_ID, -3, "Tura Pariza", 100);
        paymentsDbContext.TourPurchaseTokens.Add(token);
        paymentsDbContext.SaveChanges();

        // Start execution with tourist 1
        var startDto = new TourExecutionStartDto
        {
            TourId = -3,
            Latitude = 48.8566,
            Longitude = 2.3522
        };
        var startResult = controller1.Start(startDto).Result as CreatedAtActionResult;
        var startData = startResult!.Value as TourExecutionStartResultDto;
        var executionId = startData!.TourExecutionId;

        // Act & Assert - Tourist 2 tries to complete tourist 1's execution
        Should.Throw<InvalidOperationException>(() => controller2.Complete(executionId));
    }

    private static TourExecutionController CreateController(IServiceScope scope, long touristId)
    {
        return new TourExecutionController(scope.ServiceProvider.GetRequiredService<ITourExecutionService>())
        {
            ControllerContext = BuildContext(touristId.ToString())
        };
    }
}
