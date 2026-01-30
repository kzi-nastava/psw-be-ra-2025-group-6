using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Encounters.Tests.Integration;

[Collection("Sequential")]
public class KeyPointChallengeIntegrationTests : BaseEncountersIntegrationTest
{
    public KeyPointChallengeIntegrationTests(EncountersTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates_challenge_for_keypoint()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IChallengePublicService>();

        var dto = new ChallengeDto
        {
            Title = "Test KeyPoint Challenge",
            Description = "Challenge for testing KeyPoint relation",
            XP = 75,
            Type = "Location",
            IsRequiredForSecret = true,
            ActivationRadiusMeters = 50
        };

        long keyPointId = -11; // From Tours seed data
        long authorId = -1; // Test author
        double longitude = 2.2945;
        double latitude = 48.8584;

        // Act
        var result = service.CreateForKeyPoint(dto, keyPointId, longitude, latitude, authorId);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Title.ShouldBe(dto.Title);
        result.KeyPointId.ShouldBe(keyPointId);
        result.IsRequiredForSecret.ShouldBe(true);
        result.Longitude.ShouldBe(longitude);
        result.Latitude.ShouldBe(latitude);
        result.Status.ShouldBe("Active");
        result.CreatorId.ShouldBe(authorId);
    }

    [Fact]
    public void Gets_challenges_by_keypoint_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IChallengePublicService>();
        
        long keyPointId = -11; // From seed data

        // Act
        var result = service.GetByKeyPointId(keyPointId);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);
        result.ShouldAllBe(c => c.KeyPointId == keyPointId);
        result.ShouldAllBe(c => c.Status == "Active");
    }

    [Fact]
    public void Gets_empty_list_for_keypoint_without_challenges()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IChallengePublicService>();
        
        long keyPointId = -999; // Non-existent KeyPoint

        // Act
        var result = service.GetByKeyPointId(keyPointId);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(0);
    }

    [Fact]
    public void Create_for_keypoint_fails_with_invalid_type()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IChallengePublicService>();

        var dto = new ChallengeDto
        {
            Title = "Invalid Challenge",
            Description = "Testing invalid type",
            XP = 50,
            Type = "InvalidType",
            IsRequiredForSecret = false,
            ActivationRadiusMeters = 50
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() =>
        {
            service.CreateForKeyPoint(dto, -11, 2.2945, 48.8584, -1);
        });
    }

    [Fact]
    public void Required_for_secret_flag_is_saved_correctly()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IChallengePublicService>();

        var requiredDto = new ChallengeDto
        {
            Title = "Required Challenge",
            Description = "This is required",
            XP = 100,
            Type = "Location",
            IsRequiredForSecret = true,
            ActivationRadiusMeters = 50
        };

        var optionalDto = new ChallengeDto
        {
            Title = "Optional Challenge",
            Description = "This is optional",
            XP = 50,
            Type = "Misc",
            IsRequiredForSecret = false,
            ActivationRadiusMeters = 50
        };

        long authorId = -1; // Test author

        // Act
        var requiredResult = service.CreateForKeyPoint(requiredDto, -10, 2.3376, 48.8606, authorId);
        var optionalResult = service.CreateForKeyPoint(optionalDto, -10, 2.3376, 48.8606, authorId);

        // Assert
        requiredResult.IsRequiredForSecret.ShouldBeTrue();
        requiredResult.CreatorId.ShouldBe(authorId);
        optionalResult.IsRequiredForSecret.ShouldBeFalse();
        optionalResult.CreatorId.ShouldBe(authorId);
    }
}
