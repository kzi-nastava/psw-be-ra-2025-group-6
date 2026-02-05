using Explorer.API.Controllers;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class RecommendedToursTests : BaseToursIntegrationTest
{
    public RecommendedToursTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Returns_recommended_tours_for_preferences()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = new TourRecommendationsController(scope.ServiceProvider.GetRequiredService<ITourRecommendationService>())
        {
            ControllerContext = BuildTouristContext("-21")
        };

        var result = controller.GetRecommended(6).Result as OkObjectResult;
        result.ShouldNotBeNull();
        var tours = result.Value as List<TourDto>;
        tours.ShouldNotBeNull();
        tours.ShouldContain(t => t.Id == -11);
    }

    [Fact]
    public void Returns_recommended_tour_when_preferences_match_published()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = new TourRecommendationsController(scope.ServiceProvider.GetRequiredService<ITourRecommendationService>())
        {
            ControllerContext = BuildTouristContext("-22")
        };

        var result = controller.GetRecommended(6).Result as OkObjectResult;
        result.ShouldNotBeNull();
        var tours = result.Value as List<TourDto>;
        tours.ShouldNotBeNull();
        tours.ShouldContain(t => t.Id == -13);
    }

    private static ControllerContext BuildTouristContext(string id)
    {
        return new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("id", id),
                    new Claim("personId", id),
                    new Claim(ClaimTypes.Role, "Tourist")
                }))
            }
        };
    }
}
