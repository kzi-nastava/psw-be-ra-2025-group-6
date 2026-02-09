using Explorer.API.Controllers.Author.Authoring;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Core.Domain;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Authoring;

[Collection("Sequential")]
public class TourPublishNotificationTests : BaseToursIntegrationTest
{
    public TourPublishNotificationTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Publishing_creates_notifications_for_matching_tourists()
    {
        using var scope = Factory.Services.CreateScope();
        var authorId = 3;
        var tourId = -1;

        var controller = CreateController(scope, authorId);
        var stakeholdersContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var tourDto = ((ObjectResult)controller.Get(tourId).Result)?.Value as TourDto;
        tourDto.Description = "Valid description for publish.";
        tourDto.Tags = new List<string> { "city", "river" };
        controller.Update(tourId, tourDto);

        var kp1 = new KeyPointDto { Name = "KP1", Description = "D1", Latitude = 45.1, Longitude = 19.1, ImagePath = "img1.jpg", Secret = "S1" };
        var kp2 = new KeyPointDto { Name = "KP2", Description = "D2", Latitude = 45.2, Longitude = 19.2, ImagePath = "img2.jpg", Secret = "S2" };
        controller.AddKeyPoint(tourId, kp1);
        controller.AddKeyPoint(tourId, kp2);

        var result = ((ObjectResult)controller.Publish(tourId).Result)?.Value as TourDto;
        result.ShouldNotBeNull();
        result.Status.ShouldBe(TourStatusDto.CONFIRMED);

        var notifications = stakeholdersContext.Notifications
            .Where(n => n.ReferenceId == tourId && n.Title == "New tour published")
            .ToList();

        notifications.ShouldContain(n => n.RecipientId == -21);
    }

    private static TourController CreateController(IServiceScope scope, long personId)
    {
        return new TourController(scope.ServiceProvider.GetRequiredService<ITourService>())
        {
            ControllerContext = BuildContext(personId.ToString())
        };
    }
}
