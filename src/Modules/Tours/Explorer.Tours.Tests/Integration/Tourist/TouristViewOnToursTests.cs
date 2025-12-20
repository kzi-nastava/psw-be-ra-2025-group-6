using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.UseCases.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TouristViewOnToursTests : BaseToursIntegrationTest
{
    public TouristViewOnToursTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_only_published_tours()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = controller.GetPublishedTours().Result.ShouldBeOfType<OkObjectResult>();
        var tours = result.Value as List<TouristTourDto>;

        // Assert
        tours.ShouldNotBeNull();
        tours.Count.ShouldBe(2); // Samo -3 "Tura Pariza" ima Status = 1 (PUBLISHED/CONFIRMED)
        tours.ShouldContain(t => t.Name == "Tura Pariza");
        tours.ShouldContain(t => t.Name == "Another Confirmed Tour");
    }

    [Fact]
    public void Does_not_return_draft_tours()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = controller.GetPublishedTours().Result.ShouldBeOfType<OkObjectResult>();
        var tours = result.Value as List<TouristTourDto>;

        // Assert
        tours.ShouldNotBeNull();

        // Ne sme da vrati "Tura Londona" (Id: -1, Status: 0 = DRAFT)
        tours.ShouldNotContain(t => t.Name == "Tura Londona");

        // Ne sme da vrati "Tura Beograda" (Id: -2, Status: 0 = DRAFT)
        tours.ShouldNotContain(t => t.Name == "Tura Beograda");
    }

    [Fact]
    public void Returns_complete_tour_data()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = controller.GetPublishedTours().Result.ShouldBeOfType<OkObjectResult>();
        var tours = result.Value as List<TouristTourDto>;

        // Assert
        tours.ShouldNotBeNull();
        tours.Count.ShouldBe(2);

        var tour = tours.FirstOrDefault(t => t.Name == "Tura Pariza");
        tour.Name.ShouldBe("Tura Pariza");
        tour.Description.ShouldBe("Pravo u Luvr");
        tour.Price.ShouldBe(100);
        tour.Tags.ShouldNotBeNull();
        tour.Tags.Count.ShouldBe(2);
        tour.Tags.ShouldContain("europe");
        tour.Tags.ShouldContain("7 days");
        tour.Difficulty.ShouldBe(TourDifficultyDto.EASY); // Difficulty = 0
        tour.DistanceInKm.ShouldBe(0);
        var tour2 = tours.FirstOrDefault(t => t.Name == "Another Confirmed Tour");
        tour2.Name.ShouldBe("Another Confirmed Tour");
        tour2.Description.ShouldBe("Konfirmovana tura");
        tour2.Price.ShouldBe(100);
        tour2.Tags.ShouldNotBeNull();
        tour2.Tags.Count.ShouldBe(2);
        tour2.Tags.ShouldContain("Confirmed Tour");
        tour2.Tags.ShouldContain("7 days");
        tour2.Difficulty.ShouldBe(TourDifficultyDto.EASY); // Difficulty = 0
        tour2.DistanceInKm.ShouldBe(0);
    }

    [Fact]
    public void Returns_tour_with_duration_list()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = controller.GetPublishedTours().Result.ShouldBeOfType<OkObjectResult>();
        var tours = result.Value as List<TouristTourDto>;

        // Assert
        tours.ShouldNotBeNull();
        tours[0].Duration.ShouldNotBeNull();
        tours[0].Duration.ShouldBeOfType<List<TourDurationDto>>();
    }

    private static TouristViewController CreateController(IServiceScope scope)
    {
        return new TouristViewController(scope.ServiceProvider.GetRequiredService<ITouristViewService>())
        {
            ControllerContext = BuildContext("1")
        };
    }
}