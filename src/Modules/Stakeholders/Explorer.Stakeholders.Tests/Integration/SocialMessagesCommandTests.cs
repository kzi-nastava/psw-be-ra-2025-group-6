using Explorer.API.Controllers;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration;

[Collection("Sequential")]
public class SocialMessagesCommandTests : BaseStakeholdersIntegrationTest
{
    public SocialMessagesCommandTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public async Task Create_message_succeeds()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -21);

        var message = new SocialMessageDto
        {
            ReceiverId = -22,
            Content = "test"
        };

        var actionResult = await controller.Create(message);

        actionResult.ShouldNotBeNull();

        var okResult = actionResult.Result as OkObjectResult;
        okResult.ShouldNotBeNull();
        okResult.StatusCode.ShouldBe(200);

        var createdMessage = okResult.Value as SocialMessageDto;
        createdMessage.ShouldNotBeNull();
        createdMessage.Content.ShouldBe("test");
        createdMessage.SenderId.ShouldBe(-21);
        createdMessage.ReceiverId.ShouldBe(-22);
    }

    private static SocialMessageController CreateController(IServiceScope scope, long personId)
    {
        return new SocialMessageController(
            scope.ServiceProvider.GetRequiredService<ISocialMessageService>())
        {
            ControllerContext = BuildContext(personId.ToString())
        };
    }
}
