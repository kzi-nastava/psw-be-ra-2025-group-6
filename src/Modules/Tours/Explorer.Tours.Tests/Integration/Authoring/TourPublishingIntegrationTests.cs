using Explorer.API.Controllers.Author.Authoring;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Authoring;

[Collection("Sequential")]
public class TourPublishingIntegrationTests : BaseToursIntegrationTest
{
    public TourPublishingIntegrationTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Publish_Succeeds()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var authorId = 3;
        var tourId = -2;

        var controller = CreateController(scope, authorId);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // 1. KORAK: Priprema podataka
        var tourDto = ((ObjectResult)controller.Get(tourId).Result)?.Value as TourDto;
        tourDto.Description = "Validan opis za testiranje objave.";
        tourDto.Tags = new List<string> { "city", "history" };
        controller.Update(tourId, tourDto);

        // 2. KORAK: Dodavanje ključnih tačaka
        var kp1 = new KeyPointDto { Name = "KP1", Description = "D1", Latitude = 45.1, Longitude = 19.1, ImagePath = "img1.jpg", Secret = "S1" };
        var kp2 = new KeyPointDto { Name = "KP2", Description = "D2", Latitude = 45.2, Longitude = 19.2, ImagePath = "img2.jpg", Secret = "S2" };

        controller.AddKeyPoint(tourId, kp1);
        controller.AddKeyPoint(tourId, kp2);

        // Act
        var result = (ObjectResult)controller.Publish(tourId).Result;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        var resultValue = result.Value as TourDto;
        resultValue.Status.ShouldBe(TourStatusDto.CONFIRMED);
        resultValue.PublishedTime.ShouldNotBeNull();

        // Assert - Database
        var storedTour = dbContext.Tours.First(t => t.Id == tourId);
        storedTour.Status.ShouldBe(TourStatus.CONFIRMED);
    }

    [Fact]
    public void Publish_Fails_NotEnoughKeyPoints()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var authorId = 3;
        var tourId = -2;

        var controller = CreateController(scope, authorId);

        var tourDto = ((ObjectResult)controller.Get(tourId).Result)?.Value as TourDto;
        tourDto.Description = "Validan opis, ali nema tacaka.";
        tourDto.Tags = new List<string> { "test" };
        controller.Update(tourId, tourDto);

        // Act
        var result = controller.Publish(tourId).Result as BadRequestObjectResult;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(400);

        var json = result.Value.ToString();
        json.ShouldContain("Tour must have at least two key points");
    }

    [Fact]
    public void Publish_Fails_InvalidStatus()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var authorId = 4;
        var tourId = -3; // Tura -3 je već CONFIRMED

        var controller = CreateController(scope, authorId);

        // Act
        var result = controller.Publish(tourId).Result as BadRequestObjectResult;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(400);
        result.Value.ToString().ShouldContain("Only draft tours can be published");
    }

    [Fact]
    public void Publish_Fails_For_Non_Owner()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var intruderId = 99;
        var tourId = -1;

        var controller = CreateController(scope, intruderId);

        // Act
        var result = controller.Publish(tourId).Result as ObjectResult;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(403);
        result.Value.ToString().ShouldContain("Only the owner can publish the tour");
    }

    private static TourController CreateController(IServiceScope scope, long personId)
    {
        return new TourController(scope.ServiceProvider.GetRequiredService<ITourService>())
        {
            ControllerContext = BuildContext(personId.ToString())
        };
    }
}