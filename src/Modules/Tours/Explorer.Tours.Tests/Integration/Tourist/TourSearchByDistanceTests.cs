using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Marketplace;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Linq;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourSearchByDistanceTests : BaseToursIntegrationTest
{
    public TourSearchByDistanceTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Returns_published_tour_inside_radius()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var request = new TourSearchByDistanceRequestDto { Latitude = 45.2671, Longitude = 19.8335, DistanceInKm = 5 };

        var actionResult = controller.SearchByDistance(request);
        var okResult = actionResult.Result.ShouldBeOfType<OkObjectResult>();
        var result = okResult.Value as List<TourSummaryDto>;
        result.ShouldNotBeNull();
        result.Any(t => t.Id == -101).ShouldBeTrue();
    }

    [Fact]
    public void Filters_out_published_tour_outside_radius()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var request = new TourSearchByDistanceRequestDto { Latitude = 45.2671, Longitude = 19.8335, DistanceInKm = 5 };

        var actionResult = controller.SearchByDistance(request);
        var okResult = actionResult.Result.ShouldBeOfType<OkObjectResult>();
        var result = okResult.Value as List<TourSummaryDto>;
        result.ShouldNotBeNull();
        result.Any(t => t.Id == -102).ShouldBeFalse();
    }

    [Fact]
    public void Ignores_non_published_tour_even_if_close()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var request = new TourSearchByDistanceRequestDto { Latitude = 45.2671, Longitude = 19.8335, DistanceInKm = 5 };

        var actionResult = controller.SearchByDistance(request);
        var okResult = actionResult.Result.ShouldBeOfType<OkObjectResult>();
        var result = okResult.Value as List<TourSummaryDto>;
        result.ShouldNotBeNull();
        result.Any(t => t.Id == -103).ShouldBeFalse();
    }

    [Fact]
    public void Rejects_negative_distance()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var request = new TourSearchByDistanceRequestDto { Latitude = 45.0, Longitude = 19.0, DistanceInKm = -1 };

        var actionResult = controller.SearchByDistance(request);
        actionResult.Result.ShouldBeOfType<BadRequestObjectResult>();
    }

    private static TourMarketplaceController CreateController(IServiceScope scope) =>
        new(scope.ServiceProvider.GetRequiredService<ITourMarketplaceService>())
        {
            ControllerContext = BuildContext("5")
        };
}
