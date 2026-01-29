using Explorer.API.Controllers;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration;

[Collection("Sequential")]
public class FollowCommandTests : BaseStakeholdersIntegrationTest
{
    public FollowCommandTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Follow_succeeds()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -21);
        long followedId = -22;

        var result = controller.Follow(followedId).Result as OkObjectResult;

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        var followDto = result.Value as FollowDto;
        followDto.ShouldNotBeNull();
        followDto.FollowedId.ShouldBe(followedId);
    }

    [Fact]
    public void Unfollow_succeeds()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -21);
        long followedId = -22;

        controller.Follow(followedId);

        var result = controller.Unfollow(followedId) as NoContentResult;

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(204);
    }

    [Fact]
    public void Retrieves_followers_count()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -21);
        long userId = -22;

        var result = controller.GetFollowersCount(userId).Result as OkObjectResult;

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        var count = (int)result.Value;
        count.ShouldBeGreaterThanOrEqualTo(0);
    }

    private static FollowController CreateController(IServiceScope scope, long personId)
    {
        return new FollowController(scope.ServiceProvider.GetRequiredService<IFollowService>())
        {
            ControllerContext = BuildContext(personId.ToString())
        };
    }
}