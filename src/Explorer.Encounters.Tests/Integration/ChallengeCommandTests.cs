using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace Explorer.Encounters.Tests.Integration;

[Collection("Sequential")]
public class ChallengeCommandTests : BaseEncountersIntegrationTest
{
    public ChallengeCommandTests(EncountersTestFactory factory) : base(factory) { }

    [Fact]
    public void Admin_Can_Create_Update_Delete_Challenge()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = new Explorer.API.Controllers.Encounters.ChallengesController(
            scope.ServiceProvider.GetRequiredService<Explorer.Encounters.API.Public.IChallengePublicService>(),
            scope.ServiceProvider.GetRequiredService<Explorer.Encounters.Core.UseCases.IChallengeService>());

        // Create
        var newDto = new ChallengeDto { Title = "Test Create", Description = "Desc", Longitude = 10, Latitude = 10, XP = 100, Status = "Draft", Type = "Location" };
        var created = ((ObjectResult)controller.Create(newDto).Result).Value as ChallengeDto;
        created.ShouldNotBeNull();
        created.Id.ShouldNotBe(0);

        // Update
        var updateDto = new ChallengeDto { Title = "Updated", Description = "Desc2", Longitude = 11, Latitude = 11, XP = 150, Status = "Active", Type = "Location" };
        var updated = ((ObjectResult)controller.Update(created.Id, updateDto).Result).Value as ChallengeDto;
        updated.ShouldNotBeNull();
        updated.Title.ShouldBe("Updated");
        updated.Status.ShouldBe("Active");

        // Delete
        var deleteResult = controller.Delete(created.Id);
        deleteResult.ShouldBeOfType<OkResult>();

        var db = scope.ServiceProvider.GetRequiredService<EncountersContext>();
        var stored = db.Challenges.Find(created.Id);
        stored.ShouldBeNull();
    }
}
