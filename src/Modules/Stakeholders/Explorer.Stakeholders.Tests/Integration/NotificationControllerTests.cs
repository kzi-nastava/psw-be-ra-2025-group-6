using Explorer.API.Controllers;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration;

[Collection("Sequential")]
public class NotificationControllerTests : BaseStakeholdersIntegrationTest
{
    public NotificationControllerTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Get_notifications_returns_for_recipient()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -21);

        var result = controller.Get() as OkObjectResult;

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        var notifications = result.Value as List<NotificationDto>;
        notifications.ShouldNotBeNull();
        notifications.ShouldContain(n => n.RecipientId == -21);
    }

    [Fact]
    public void Mark_as_read_updates_status()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -21);

        var result = controller.MarkAsReadLegacy(-1001) as OkObjectResult;

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        var notification = result.Value as NotificationDto;
        notification.ShouldNotBeNull();
        notification.IsRead.ShouldBeTrue();
        notification.Status.ShouldBe("Read");
    }

    private static NotificationController CreateController(IServiceScope scope, long personId)
    {
        return new NotificationController(
            scope.ServiceProvider.GetRequiredService<INotificationService>(),
            scope.ServiceProvider.GetRequiredService<Explorer.Tours.API.Public.Tourist.ITourRecommendationService>(),
            scope.ServiceProvider.GetRequiredService<Explorer.Tours.API.Public.Tourist.ITouristPreferencesService>())
        {
            ControllerContext = BuildContext(personId.ToString())
        };
    }
}
