using Explorer.API.Controllers;
using Explorer.API.Controllers.Tourist;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TouristRecommendationsIntegrationTests : BaseToursIntegrationTest
{
    public TouristRecommendationsIntegrationTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Get_recommendations_returns_new_ids()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateRecommendationsController(scope, "-21");

        var result = controller.Get(10).Result as OkObjectResult;
        result.ShouldNotBeNull();

        var payload = result.Value as TourRecommendationsDto;
        payload.ShouldNotBeNull();
        payload.TourIds.ShouldContain(-11);
        payload.NewTourIds.ShouldContain(-11);
    }

    [Fact]
    public void Acknowledge_clears_new_recommendations()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateRecommendationsController(scope, "-21");

        var initial = controller.Get(10).Result as OkObjectResult;
        var initialPayload = initial?.Value as TourRecommendationsDto;
        initialPayload.ShouldNotBeNull();
        initialPayload.NewTourIds.ShouldContain(-11);

        var ackResult = controller.Acknowledge();
        ackResult.ShouldBeOfType<NoContentResult>();

        var afterAck = controller.Get(10).Result as OkObjectResult;
        var afterAckPayload = afterAck?.Value as TourRecommendationsDto;
        afterAckPayload.ShouldNotBeNull();
        afterAckPayload.NewTourIds.Count.ShouldBe(0);
    }

    [Fact]
    public void Notification_created_on_first_fetch_and_deduped()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateNotificationController(scope, "-21");

        var first = controller.GetUnread().Result as OkObjectResult;
        first.ShouldNotBeNull();
        var firstList = first.Value as List<NotificationDto>;
        firstList.ShouldNotBeNull();
        firstList.Count.ShouldBe(1);
        firstList[0].Title.ShouldBe("New tour recommendations");
        firstList[0].Content.ShouldContain("Added");

        var second = controller.GetUnread().Result as OkObjectResult;
        second.ShouldNotBeNull();
        var secondList = second.Value as List<NotificationDto>;
        secondList.ShouldNotBeNull();
        secondList.Count.ShouldBe(1);
    }

    private static TouristRecommendationsController CreateRecommendationsController(IServiceScope scope, string id)
    {
        return new TouristRecommendationsController(
            scope.ServiceProvider.GetRequiredService<ITourRecommendationService>(),
            scope.ServiceProvider.GetRequiredService<ITouristPreferencesService>())
        {
            ControllerContext = BuildTouristContext(id)
        };
    }

    private static NotificationController CreateNotificationController(IServiceScope scope, string id)
    {
        return new NotificationController(
            scope.ServiceProvider.GetRequiredService<INotificationService>(),
            scope.ServiceProvider.GetRequiredService<ITourRecommendationService>(),
            scope.ServiceProvider.GetRequiredService<ITouristPreferencesService>())
        {
            ControllerContext = BuildTouristContext(id)
        };
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
