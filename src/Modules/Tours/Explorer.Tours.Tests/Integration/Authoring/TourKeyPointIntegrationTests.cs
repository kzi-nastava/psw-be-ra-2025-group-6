using Explorer.API.Controllers.Author.Authoring;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using Explorer.Tours.Core.Domain; // Dodato za TourStatus enum ako zatreba

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

        // PREMA TVOM SQL-u: Tura -1 je Draft i vlasnik je 3
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
        result.KeyPoints.Count.ShouldBe(1); // Ocekujemo 1 jer je tura bila prazna
        result.KeyPoints[0].Name.ShouldBe(keyPointDto.Name);

        // Assert - Database
        var storedTour = dbContext.Tours.First(t => t.Id == tourId);
        storedTour.KeyPoints.ShouldNotBeNull();
        storedTour.KeyPoints.Count.ShouldBe(1);
    }

    [Fact]
    public void Add_KeyPoints_Calculates_Distance()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        // Koristimo istu turu (-1), testovi se izvrsavaju transakciono (rollback na kraju)
        var authorId = 3;
        var tourId = -1;

        var controller = CreateController(scope, authorId);

        // Tacka A
        var point1 = new KeyPointDto
        {
            Name = "Tacka A",
            Description = "A",
            ImagePath = "img.jpg",
            Secret = "S",
            Latitude = 45.2396,
            Longitude = 19.8227
        };
        // Tacka B (malo dalje, oko 1.3km)
        var point2 = new KeyPointDto
        {
            Name = "Tacka B",
            Description = "B",
            ImagePath = "img.jpg",
            Secret = "S",
            Latitude = 45.2500,
            Longitude = 19.8300
        };

        // Act
        controller.AddKeyPoint(tourId, point1); // Prva tačka
        var result = ((ObjectResult)controller.AddKeyPoint(tourId, point2).Result)?.Value as TourDto; // Druga tačka

        // Assert
        result.ShouldNotBeNull();
        result.KeyPoints.Count.ShouldBe(2);

        // Distanca mora biti veća od 0
        result.DistanceInKm.ShouldBeGreaterThan(0.5);
    }

    [Fact]
    public void Add_KeyPoint_Fails_For_Non_Owner()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var tourId = -1; // Vlasnik je 3
        var intruderId = 99; // Neki nepoznati korisnik

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

        // Act & Assert
        Should.Throw<ForbiddenException>(() =>
        {
            controller.AddKeyPoint(tourId, keyPointDto);
        });
    }

    [Fact]
    public void Add_KeyPoint_Fails_For_Archived_Tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();

        // PREMA TVOM SQL-u: Tura -3 je Status 1 (Confirmed) i vlasnik je 4
        var authorId = 4;
        var tourId = -3;

        var controller = CreateController(scope, authorId);

        // Prvo moramo arhivirati turu da bismo testirali zabranu menjanja arhivirane ture.
        // Tura je u statusu 1 (Confirmed), tako da možemo pozvati Archive().
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
        // Očekujemo grešku jer je tura sada ARCHIVED
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