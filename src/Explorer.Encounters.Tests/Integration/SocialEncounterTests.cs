using Explorer.API.Controllers.Tourist;
using Explorer.API.Controllers.Administrator;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Linq;

namespace Explorer.Encounters.Tests.Integration;

[Collection("Sequential")]
public class SocialEncounterTests : BaseEncountersIntegrationTest
{
    public SocialEncounterTests(EncountersTestFactory factory) : base(factory) { }

    [Fact]
    public void Admin_creates_social_encounter()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAdminController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

        // Kreiraj novi Social Challenge za test
        var challenge = new Challenge("Test Social Encounter", "Test description", 19.845, 45.267, 100, ChallengeType.Social, ChallengeStatus.Active);
        dbContext.Challenges.Add(challenge);
        dbContext.SaveChanges();

        var newSocialEncounter = new SocialEncounterDto
        {
            ChallengeId = challenge.Id,
            RequiredPeople = 5,
            RadiusMeters = 15.0
        };

        // Act
        var result = ((ObjectResult)controller.Create(newSocialEncounter).Result)?.Value as SocialEncounterDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.ChallengeId.ShouldBe(challenge.Id);
        result.RequiredPeople.ShouldBe(5);
        result.RadiusMeters.ShouldBe(15.0);

        var stored = dbContext.SocialEncounters.FirstOrDefault(se => se.ChallengeId == challenge.Id);
        stored.ShouldNotBeNull();
        stored.RequiredPeople.ShouldBe(5);
    }

    [Fact]
    public void Admin_updates_social_encounter()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAdminController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

        // Kreiraj Challenge i SocialEncounter
        var challenge = new Challenge("Update Test", "Description", 19.845, 45.267, 100, ChallengeType.Social, ChallengeStatus.Active);
        dbContext.Challenges.Add(challenge);
        dbContext.SaveChanges();

        var newSocialEncounter = new SocialEncounterDto
        {
            ChallengeId = challenge.Id,
            RequiredPeople = 3,
            RadiusMeters = 10.0
        };
        var created = ((ObjectResult)controller.Create(newSocialEncounter).Result)?.Value as SocialEncounterDto;

        var updateDto = new SocialEncounterDto
        {
            Id = created.Id,
            ChallengeId = challenge.Id,
            RequiredPeople = 7,
            RadiusMeters = 20.0
        };

        // Act
        var result = ((ObjectResult)controller.Update(created.Id, updateDto).Result)?.Value as SocialEncounterDto;

        // Assert
        result.ShouldNotBeNull();
        result.RequiredPeople.ShouldBe(7);
        result.RadiusMeters.ShouldBe(20.0);

        var stored = dbContext.SocialEncounters.FirstOrDefault(se => se.Id == created.Id);
        stored.ShouldNotBeNull();
        stored.RequiredPeople.ShouldBe(7);
    }

    [Fact]
    public void Admin_deletes_social_encounter()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAdminController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

        var challenge = new Challenge("Delete Test", "Description", 19.845, 45.267, 100, ChallengeType.Social, ChallengeStatus.Active);
        dbContext.Challenges.Add(challenge);
        dbContext.SaveChanges();

        var newSocialEncounter = new SocialEncounterDto
        {
            ChallengeId = challenge.Id,
            RequiredPeople = 4,
            RadiusMeters = 12.0
        };
        var created = ((ObjectResult)controller.Create(newSocialEncounter).Result)?.Value as SocialEncounterDto;

        // Act
        var result = (NoContentResult)controller.Delete(created.Id);

        // Assert
        result.StatusCode.ShouldBe(204);

        var stored = dbContext.SocialEncounters.FirstOrDefault(se => se.Id == created.Id);
        stored.ShouldBeNull();
    }

    [Fact]
    public void Admin_cannot_create_duplicate_social_encounter()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAdminController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

        var challenge = new Challenge("Duplicate Test", "Description", 19.845, 45.267, 100, ChallengeType.Social, ChallengeStatus.Active);
        dbContext.Challenges.Add(challenge);
        dbContext.SaveChanges();

        var socialEncounter = new SocialEncounterDto
        {
            ChallengeId = challenge.Id,
            RequiredPeople = 5,
            RadiusMeters = 15.0
        };
        controller.Create(socialEncounter);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => controller.Create(socialEncounter));
    }

    [Fact]
    public void Tourist_activates_social_encounter_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var adminController = CreateAdminController(scope);
        var touristController = CreateTouristController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

        // Kreiraj Challenge i Social Encounter
        var challenge = new Challenge("Activation Test", "Description", 45.2671, 19.8453, 100, ChallengeType.Social, ChallengeStatus.Active);
        dbContext.Challenges.Add(challenge);
        dbContext.SaveChanges();

        var socialEncounter = new SocialEncounterDto
        {
            ChallengeId = challenge.Id,
            RequiredPeople = 2,
            RadiusMeters = 50.0
        };
        adminController.Create(socialEncounter);

        var activateRequest = new ActivateSocialEncounterRequestDto
        {
            CurrentLatitude = 45.2671, // Ista lokacija kao Challenge
            CurrentLongitude = 19.8453
        };

        // Act
        var result = ((ObjectResult)touristController.ActivateSocialEncounter(challenge.Id, activateRequest).Result)?.Value
            as ActivateSocialEncounterResponseDto;

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();
        result.IsWithinRadius.ShouldBeTrue();
        result.RequiredPeople.ShouldBe(2);
        result.CurrentActiveCount.ShouldBeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public void Tourist_activation_fails_when_too_far()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var adminController = CreateAdminController(scope);
        var touristController = CreateTouristController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

        var challenge = new Challenge("Distance Test", "Description", 45.2671, 19.8453, 100, ChallengeType.Social, ChallengeStatus.Active);
        dbContext.Challenges.Add(challenge);
        dbContext.SaveChanges();

        var socialEncounter = new SocialEncounterDto
        {
            ChallengeId = challenge.Id,
            RequiredPeople = 2,
            RadiusMeters = 10.0
        };
        adminController.Create(socialEncounter);

        var activateRequest = new ActivateSocialEncounterRequestDto
        {
            CurrentLatitude = 40.0, // Daleko od Challenge lokacije
            CurrentLongitude = 20.0
        };

        // Act
        var result = ((ObjectResult)touristController.ActivateSocialEncounter(challenge.Id, activateRequest).Result)?.Value
            as ActivateSocialEncounterResponseDto;

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeFalse();
        result.IsWithinRadius.ShouldBeFalse();
        result.Message.ShouldContain("too far");
    }

    [Fact]
    public void Tourist_sends_heartbeat_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var adminController = CreateAdminController(scope);
        var touristController = CreateTouristController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

        var challenge = new Challenge("Heartbeat Test", "Description", 45.2671, 19.8453, 100, ChallengeType.Social, ChallengeStatus.Active);
        dbContext.Challenges.Add(challenge);
        dbContext.SaveChanges();

        var socialEncounter = new SocialEncounterDto
        {
            ChallengeId = challenge.Id,
            RequiredPeople = 2,
            RadiusMeters = 50.0
        };
        adminController.Create(socialEncounter);

        var activateRequest = new ActivateSocialEncounterRequestDto
        {
            CurrentLatitude = 45.2671,
            CurrentLongitude = 19.8453
        };
        touristController.ActivateSocialEncounter(challenge.Id, activateRequest);

        var heartbeatRequest = new SocialEncounterHeartbeatRequestDto
        {
            CurrentLatitude = 45.2672, // Malo pomereno, ali i dalje u radijusu
            CurrentLongitude = 19.8454
        };

        // Act
        var result = ((ObjectResult)touristController.SendHeartbeat(challenge.Id, heartbeatRequest).Result)?.Value
            as SocialEncounterHeartbeatResponseDto;

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();
        result.StillInRadius.ShouldBeTrue();
        result.CurrentActiveCount.ShouldBeGreaterThanOrEqualTo(1);
        result.IsCompleted.ShouldBeFalse(); // Još nema dovoljno ljudi
    }

    [Fact]
    public void Tourist_heartbeat_fails_when_leaves_radius()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var adminController = CreateAdminController(scope);
        var touristController = CreateTouristController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

        var challenge = new Challenge("Leave Radius Test", "Description", 45.2671, 19.8453, 100, ChallengeType.Social, ChallengeStatus.Active);
        dbContext.Challenges.Add(challenge);
        dbContext.SaveChanges();

        var socialEncounter = new SocialEncounterDto
        {
            ChallengeId = challenge.Id,
            RequiredPeople = 2,
            RadiusMeters = 10.0
        };
        adminController.Create(socialEncounter);

        var activateRequest = new ActivateSocialEncounterRequestDto
        {
            CurrentLatitude = 45.2671,
            CurrentLongitude = 19.8453
        };
        touristController.ActivateSocialEncounter(challenge.Id, activateRequest);

        var heartbeatRequest = new SocialEncounterHeartbeatRequestDto
        {
            CurrentLatitude = 45.2700, // Predaleko
            CurrentLongitude = 19.8500
        };

        // Act
        var result = ((ObjectResult)touristController.SendHeartbeat(challenge.Id, heartbeatRequest).Result)?.Value
            as SocialEncounterHeartbeatResponseDto;

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();
        result.StillInRadius.ShouldBeFalse();
        result.Message.ShouldContain("left");
    }

    [Fact]
    public void Tourist_deactivates_social_encounter()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var adminController = CreateAdminController(scope);
        var touristController = CreateTouristController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

        var challenge = new Challenge("Deactivate Test", "Description", 45.2671, 19.8453, 100, ChallengeType.Social, ChallengeStatus.Active);
        dbContext.Challenges.Add(challenge);
        dbContext.SaveChanges();

        var socialEncounter = new SocialEncounterDto
        {
            ChallengeId = challenge.Id,
            RequiredPeople = 2,
            RadiusMeters = 50.0
        };
        var created = ((ObjectResult)adminController.Create(socialEncounter).Result)?.Value as SocialEncounterDto;

        var activateRequest = new ActivateSocialEncounterRequestDto
        {
            CurrentLatitude = 45.2671,
            CurrentLongitude = 19.8453
        };
        touristController.ActivateSocialEncounter(challenge.Id, activateRequest);

        // Act
        var result = ((ObjectResult)touristController.DeactivateSocialEncounter(challenge.Id).Result)?.Value
            as DeactivateSocialEncounterResponseDto;

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();

        var socialEncounterEntity = dbContext.SocialEncounters.FirstOrDefault(se => se.Id == created.Id);
        var participants = dbContext.ActiveSocialParticipants
            .Where(p => p.SocialEncounterId == socialEncounterEntity.Id)
            .ToList();
        participants.ShouldBeEmpty();
    }

    [Fact]
    public void Tourist_cannot_activate_already_completed_encounter()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var adminController = CreateAdminController(scope);
        var touristController = CreateTouristController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

        var challenge = new Challenge("Already Completed Test", "Description", 45.2671, 19.8453, 100, ChallengeType.Social, ChallengeStatus.Active);
        dbContext.Challenges.Add(challenge);
        dbContext.SaveChanges();

        var socialEncounter = new SocialEncounterDto
        {
            ChallengeId = challenge.Id,
            RequiredPeople = 2,
            RadiusMeters = 50.0
        };
        adminController.Create(socialEncounter);

        // Simuliraj da je turista već completed ovaj challenge
        // Koristimo userId 1 umesto -1
        var completion = new EncounterCompletion(1, challenge.Id, 100);
        dbContext.EncounterCompletions.Add(completion);
        dbContext.SaveChanges();

        // Kreiraj novi kontroler sa userId 1
        var touristController2 = new SocialEncounterController(scope.ServiceProvider.GetRequiredService<ISocialEncounterService>())
        {
            ControllerContext = BuildContext("1") // Koristimo userId 1
        };

        var activateRequest = new ActivateSocialEncounterRequestDto
        {
            CurrentLatitude = 45.2671,
            CurrentLongitude = 19.8453
        };

        // Act
        var result = ((ObjectResult)touristController2.ActivateSocialEncounter(challenge.Id, activateRequest).Result)?.Value
            as ActivateSocialEncounterResponseDto;

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeFalse();
        result.Message.ShouldContain("already completed");
    }

    private static AdminSocialEncounterController CreateAdminController(IServiceScope scope)
    {
        return new AdminSocialEncounterController(scope.ServiceProvider.GetRequiredService<ISocialEncounterService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }

    private static SocialEncounterController CreateTouristController(IServiceScope scope)
    {
        return new SocialEncounterController(scope.ServiceProvider.GetRequiredService<ISocialEncounterService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}