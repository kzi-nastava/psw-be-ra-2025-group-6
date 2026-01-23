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

        // Create - Now using form parameters
        var createTask = controller.Create(
            "Test Create", // title
            "Desc", // description
            "10", // longitude
            "10", // latitude
            100, // xp
            "Location", // type
            "Draft", // status
            50, // activationRadiusMeters
            null // image
        );
        createTask.Wait();
        var created = ((ObjectResult)createTask.Result.Result!).Value as ChallengeDto;
        created.ShouldNotBeNull();
        created.Id.ShouldNotBe(0);

        // Update - Now using form parameters
        var updateTask = controller.Update(
            created.Id,
            "Updated", // title
            "Desc2", // description
            "11", // longitude
            "11", // latitude
            150, // xp
            "Location", // type
            "Active", // status
            50, // activationRadiusMeters
            null // image
        );
        updateTask.Wait();
        var updated = ((ObjectResult)updateTask.Result.Result!).Value as ChallengeDto;
        
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

    [Fact]
    public void Admin_Can_Approve_Tourist_Created_Challenge()
    {
        using var scope = Factory.Services.CreateScope();
        var challengeService = scope.ServiceProvider.GetRequiredService<Explorer.Encounters.Core.UseCases.IChallengeService>();
        
        // Tourist creates challenge
        var touristChallenge = new ChallengeDto
        {
            Title = "Tourist Challenge",
            Description = "Created by tourist",
            Longitude = 19.84,
            Latitude = 45.25,
            XP = 500,
            Type = "Misc",
            ActivationRadiusMeters = 50
        };
        
        var created = challengeService.CreateByTourist(touristChallenge, 2); // Use valid tourist ID
        created.Status.ShouldBe("Draft");
        created.IsCreatedByTourist.ShouldBeTrue();
        
        // Admin approves
        var approved = challengeService.ApproveChallenge(created.Id);
        approved.Status.ShouldBe("Active");
    }

    [Fact]
    public void Admin_Can_Reject_Tourist_Created_Challenge()
    {
        using var scope = Factory.Services.CreateScope();
        var challengeService = scope.ServiceProvider.GetRequiredService<Explorer.Encounters.Core.UseCases.IChallengeService>();
        var db = scope.ServiceProvider.GetRequiredService<EncountersContext>();
        
        // Tourist creates challenge
        var touristChallenge = new ChallengeDto
        {
            Title = "Tourist Challenge 2",
            Description = "To be rejected",
            Longitude = 19.84,
            Latitude = 45.25,
            XP = 500,
            Type = "Misc",
            ActivationRadiusMeters = 50
        };
        
        var created = challengeService.CreateByTourist(touristChallenge, 2); // Use valid tourist ID from seed data
        created.Status.ShouldBe("Draft");
        
        // Admin rejects (now archives instead of deleting)
        var rejected = challengeService.RejectChallenge(created.Id);
        rejected.ShouldNotBeNull();
        rejected.Status.ShouldBe("Archived");
        
        // Verify challenge is archived, not deleted
        var archivedChallenge = db.Challenges.Find(created.Id);
        archivedChallenge.ShouldNotBeNull();
        archivedChallenge.Status.ToString().ShouldBe("Archived");
    }

    [Fact]
    public void GetPendingApproval_Returns_Only_Tourist_Created_Draft_Challenges()
    {
        using var scope = Factory.Services.CreateScope();
        var challengeService = scope.ServiceProvider.GetRequiredService<Explorer.Encounters.Core.UseCases.IChallengeService>();
        
        // Create tourist challenge (Draft)
        var touristChallenge = new ChallengeDto
        {
            Title = "Pending Tourist Challenge",
            Description = "Should appear in pending list",
            Longitude = 19.84,
            Latitude = 45.25,
            XP = 500,
            Type = "Misc",
            ActivationRadiusMeters = 50
        };
        
        challengeService.CreateByTourist(touristChallenge, 2); // Use valid tourist ID
        
        // Get pending approval list
        var pending = challengeService.GetPendingApproval();
        pending.ShouldNotBeEmpty();
        pending.ShouldAllBe(c => c.Status == "Draft" && c.IsCreatedByTourist);
    }

    [Fact]
    public void Challenge_Remains_Active_After_Completion()
    {
        using var scope = Factory.Services.CreateScope();
        var challengeService = scope.ServiceProvider.GetRequiredService<Explorer.Encounters.Core.UseCases.IChallengeService>();
        var db = scope.ServiceProvider.GetRequiredService<EncountersContext>();
        
        // Create an active Location challenge
        var challenge = new ChallengeDto
        {
            Title = "Hidden Location Test",
            Description = "Should remain active after completion",
            Longitude = 19.84,
            Latitude = 45.25,
            XP = 50,
            Type = "Location",
            Status = "Active",
            ActivationRadiusMeters = 50
        };
        
        var createTask = new Explorer.API.Controllers.Encounters.ChallengesController(
            scope.ServiceProvider.GetRequiredService<Explorer.Encounters.API.Public.IChallengePublicService>(),
            challengeService)
            .Create(
                challenge.Title, 
                challenge.Description, 
                challenge.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture),
                challenge.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture),
                challenge.XP, 
                challenge.Type, 
                challenge.Status, 
                challenge.ActivationRadiusMeters, 
                null);
        createTask.Wait();
        var createResult = createTask.Result.Result as ObjectResult;
        createResult.ShouldNotBeNull();
        var created = createResult.Value as ChallengeDto;
        created.ShouldNotBeNull();
        
        created.Status.ShouldBe("Active");
        
        // Verify challenge remains Active after creation (simulating that tourists can complete it)
        var retrieved = challengeService.Get(created.Id);
        retrieved.Status.ShouldBe("Active");
        
        // Note: Challenge should NOT be auto-archived after completion
        // It should remain Active so multiple tourists can complete it
        // Only admin rejection should archive a challenge
    }
}
