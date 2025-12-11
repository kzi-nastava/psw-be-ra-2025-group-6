using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Shopping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Shopping;

[Collection("Sequential")]
public class TourDistanceSearchTests : BaseToursIntegrationTest
{
    public TourDistanceSearchTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Returns_published_tours_within_radius()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var response = controller.SearchByDistance(45.2671, 19.8335, 5);
        var okResult = response.Result.ShouldBeOfType<OkObjectResult>();
        var tours = okResult.Value.ShouldBeOfType<List<TourDto>>();

        tours.Count.ShouldBe(2);
        tours[0].Id.ShouldBe(-10); // Closest (distance 0)
        tours[1].Id.ShouldBe(-11);
    }

    [Fact]
    public void Ignores_non_published_tours()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Near draft tour (-1) but should not appear in marketplace search
        var response = controller.SearchByDistance(44.7866, 20.4489, 0.5);
        var okResult = response.Result.ShouldBeOfType<OkObjectResult>();
        var tours = okResult.Value.ShouldBeOfType<List<TourDto>>();

        tours.ShouldBeEmpty();
    }

    [Fact]
    public void Returns_close_tour_for_small_radius()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var response = controller.SearchByDistance(45.2671, 19.8335, 0.5);
        var okResult = response.Result.ShouldBeOfType<OkObjectResult>();
        var tours = okResult.Value.ShouldBeOfType<List<TourDto>>();

        tours.Count.ShouldBe(1);
        tours[0].Id.ShouldBe(-10);
    }

    [Fact]
    public void Returns_far_tour_for_large_radius()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var response = controller.SearchByDistance(45.2671, 19.8335, 120);
        var okResult = response.Result.ShouldBeOfType<OkObjectResult>();
        var tours = okResult.Value.ShouldBeOfType<List<TourDto>>();

        tours.ShouldContain(t => t.Id == -12);
    }

    [Fact]
    public void Returns_bad_request_for_invalid_input()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var response = controller.SearchByDistance(95, 0, 10);
        response.Result.ShouldBeOfType<BadRequestObjectResult>();
    }

    private static TourMarketplaceController CreateController(IServiceScope scope)
    {
        return new TourMarketplaceController(scope.ServiceProvider.GetRequiredService<ITourShoppingService>())
        {
            ControllerContext = BuildContext("1")
        };
    }
}
