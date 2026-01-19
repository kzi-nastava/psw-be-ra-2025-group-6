using Explorer.API.Controllers.Author.Authoring;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Tests.Integration.Authoring;

[Collection("Sequential")]
public class TourKeyPointIntegrationTests : BaseToursIntegrationTest
{
    public TourKeyPointIntegrationTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Add_KeyPoint_Succeeds()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var authorId = 3;
        var tourId = -2; 

        var controller = CreateController(scope, authorId);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var keyPointDto = new KeyPointDto
        {
            Name = "Test Tacka 1",
            Description = "Opis tacke",
            Latitude = 45.2396,
            Longitude = 19.8227,
            ImagePath = "image.jpg",
            Secret = "Tajna"
        };

        // Act
        var result = ((ObjectResult)controller.AddKeyPoint(tourId, keyPointDto).Result)?.Value as TourDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.KeyPoints.ShouldNotBeNull();
        result.KeyPoints.Count.ShouldBe(1);
        result.KeyPoints[0].Name.ShouldBe(keyPointDto.Name);

        // Assert - Database
        var storedTour = dbContext.Tours.First(t => t.Id == tourId);
        storedTour.KeyPoints.ShouldNotBeNull();
        storedTour.KeyPoints.Count.ShouldBe(1);
    }

    [Fact]
    public void Update_Distance_Succeeds()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var authorId = 3;
        var tourId = -1; 

        var controller = CreateController(scope, authorId);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        double newDistance = 5.5;

        // Act
        var result = ((ObjectResult)controller.UpdateDistance(tourId, newDistance).Result)?.Value as TourDto;

        // Assert
        result.ShouldNotBeNull();
        result.DistanceInKm.ShouldBe(newDistance); 

        // Provera u bazi
        var storedTour = dbContext.Tours.First(t => t.Id == tourId);
        storedTour.DistanceInKm.ShouldBe(newDistance);
    }

    [Fact]
    public void Add_KeyPoint_Fails_For_Non_Owner()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var tourId = -1;
        var intruderId = 99;

        var controller = CreateController(scope, intruderId);

        var keyPointDto = new KeyPointDto
        {
            Name = "Hacker Point",
            Description = "...",
            Latitude = 0,
            Longitude = 0,
            ImagePath = "x",
            Secret = "x"
        };

        // Act
        var result = controller.AddKeyPoint(tourId, keyPointDto).Result as ObjectResult;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(StatusCodes.Status403Forbidden);
    }

    [Fact]
    public void Add_KeyPoint_Fails_For_Archived_Tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var authorId = 4;
        var tourId = -3;

        var controller = CreateController(scope, authorId);

        controller.Archive(tourId);

        var keyPointDto = new KeyPointDto
        {
            Name = "Invalid Point",
            Description = "...",
            Latitude = 0,
            Longitude = 0,
            ImagePath = "x",
            Secret = "x"
        };

        // Act & Assert
        Should.Throw<InvalidOperationException>(() =>
        {
            controller.AddKeyPoint(tourId, keyPointDto);
        });
    }

    private static TourController CreateController(IServiceScope scope, long personId)
    {
        return new TourController(scope.ServiceProvider.GetRequiredService<ITourService>())
        {
            ControllerContext = BuildContext(personId.ToString())
        };
    }
}
