using Explorer.API.Controllers.Administrator;
using Explorer.API.Controllers.Tourist;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Linq;

namespace Explorer.Encounters.Tests.Integration;

[Collection("Sequential")]
public class SocialEncounterTests : BaseEncountersIntegrationTest
{
    public SocialEncounterTests(EncountersTestFactory factory) : base(factory) { }
/*
    [Fact]
    public void Debug_Check_Connection_String()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

        var connectionString = dbContext.Database.GetConnectionString();
        Console.WriteLine($"Connection String: {connectionString}");

        // Proveri da li uopšte može da se poveže
        var canConnect = dbContext.Database.CanConnect();
        Console.WriteLine($"Can Connect: {canConnect}");

        // Proveri koliko ima Challenges
        var count = dbContext.Challenges.Count();
        Console.WriteLine($"Total Challenges: {count}");
    }
    [Fact]
    public void Tourist_activates_social_encounter_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var touristController = CreateTouristController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

        // Seed-ovani SocialEncounter
        var challengeId = -4; // Activation Test

        // Provera da li podaci postoje
        var challenge = dbContext.Challenges.FirstOrDefault(c => c.Id == challengeId);
        challenge.ShouldNotBeNull($"Challenge with ID {challengeId} not found in database");

        var socialEncounter = dbContext.SocialEncounters.FirstOrDefault(se => se.ChallengeId == challengeId);
        socialEncounter.ShouldNotBeNull($"SocialEncounter for Challenge {challengeId} not found in database");

        var activateRequest = new ActivateSocialEncounterRequestDto
        {
            CurrentLatitude = 45.2671,
            CurrentLongitude = 19.8453
        };

        var result = ((ObjectResult)touristController.ActivateSocialEncounter(challengeId, activateRequest).Result)?.Value
            as ActivateSocialEncounterResponseDto;

        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();
        result.IsWithinRadius.ShouldBeTrue();
        result.RequiredPeople.ShouldBe(socialEncounter.RequiredPeople);
        result.CurrentActiveCount.ShouldBeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public void Tourist_activation_fails_when_too_far()
    {
        using var scope = Factory.Services.CreateScope();
        var touristController = CreateTouristController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

        var challengeId = -5; // Distance Test

        // Provera da li podaci postoje
        var challenge = dbContext.Challenges.FirstOrDefault(c => c.Id == challengeId);
        challenge.ShouldNotBeNull($"Challenge with ID {challengeId} not found in database");

        var socialEncounter = dbContext.SocialEncounters.FirstOrDefault(se => se.ChallengeId == challengeId);
        socialEncounter.ShouldNotBeNull($"SocialEncounter for Challenge {challengeId} not found in database");

        var activateRequest = new ActivateSocialEncounterRequestDto
        {
            CurrentLatitude = 40.0, // Daleko od lokacije
            CurrentLongitude = 20.0
        };

        var result = ((ObjectResult)touristController.ActivateSocialEncounter(challengeId, activateRequest).Result)?.Value
            as ActivateSocialEncounterResponseDto;

        result.ShouldNotBeNull();
        result.Success.ShouldBeFalse();
        result.IsWithinRadius.ShouldBeFalse();
        result.Message.ShouldContain("too far");
    }

    [Fact]
    public void Tourist_sends_heartbeat_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var touristController = CreateTouristController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

        var challengeId = -6; // Heartbeat Test

        // Provera da li podaci postoje
        var challenge = dbContext.Challenges.FirstOrDefault(c => c.Id == challengeId);
        challenge.ShouldNotBeNull($"Challenge with ID {challengeId} not found in database");

        var socialEncounter = dbContext.SocialEncounters.FirstOrDefault(se => se.ChallengeId == challengeId);
        socialEncounter.ShouldNotBeNull($"SocialEncounter for Challenge {challengeId} not found in database");

        var activateRequest = new ActivateSocialEncounterRequestDto
        {
            CurrentLatitude = 45.2671,
            CurrentLongitude = 19.8453
        };
        touristController.ActivateSocialEncounter(challengeId, activateRequest);

        var heartbeatRequest = new SocialEncounterHeartbeatRequestDto
        {
            CurrentLatitude = 45.2672,
            CurrentLongitude = 19.8454
        };

        var result = ((ObjectResult)touristController.SendHeartbeat(challengeId, heartbeatRequest).Result)?.Value
            as SocialEncounterHeartbeatResponseDto;

        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();
        result.StillInRadius.ShouldBeTrue();
        result.CurrentActiveCount.ShouldBeGreaterThanOrEqualTo(1);
        result.IsCompleted.ShouldBeFalse();
    }

    [Fact]
    public void Tourist_heartbeat_fails_when_leaves_radius()
    {
        using var scope = Factory.Services.CreateScope();
        var touristController = CreateTouristController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

        var challengeId = -7; // Leave Radius Test

        // Provera da li podaci postoje
        var challenge = dbContext.Challenges.FirstOrDefault(c => c.Id == challengeId);
        challenge.ShouldNotBeNull($"Challenge with ID {challengeId} not found in database");

        var socialEncounter = dbContext.SocialEncounters.FirstOrDefault(se => se.ChallengeId == challengeId);
        socialEncounter.ShouldNotBeNull($"SocialEncounter for Challenge {challengeId} not found in database");

        var activateRequest = new ActivateSocialEncounterRequestDto
        {
            CurrentLatitude = 45.2671,
            CurrentLongitude = 19.8453
        };
        touristController.ActivateSocialEncounter(challengeId, activateRequest);

        var heartbeatRequest = new SocialEncounterHeartbeatRequestDto
        {
            CurrentLatitude = 45.2700,
            CurrentLongitude = 19.8500
        };

        var result = ((ObjectResult)touristController.SendHeartbeat(challengeId, heartbeatRequest).Result)?.Value
            as SocialEncounterHeartbeatResponseDto;

        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();
        result.StillInRadius.ShouldBeFalse();
        result.Message.ShouldContain("left");
    }

    [Fact]
    public void Tourist_deactivates_social_encounter()
    {
        using var scope = Factory.Services.CreateScope();
        var touristController = CreateTouristController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

        var challengeId = -8; // Deactivate Test

        // Provera da li podaci postoje
        var challenge = dbContext.Challenges.FirstOrDefault(c => c.Id == challengeId);
        challenge.ShouldNotBeNull($"Challenge with ID {challengeId} not found in database");

        var socialEncounter = dbContext.SocialEncounters.FirstOrDefault(se => se.ChallengeId == challengeId);
        socialEncounter.ShouldNotBeNull($"SocialEncounter for Challenge {challengeId} not found in database");

        var activateRequest = new ActivateSocialEncounterRequestDto
        {
            CurrentLatitude = 45.2671,
            CurrentLongitude = 19.8453
        };
        touristController.ActivateSocialEncounter(challengeId, activateRequest);

        var result = ((ObjectResult)touristController.DeactivateSocialEncounter(challengeId).Result)?.Value
            as DeactivateSocialEncounterResponseDto;

        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();

        var participants = dbContext.ActiveSocialParticipants
            .Where(p => p.SocialEncounterId == socialEncounter.Id)
            .ToList();
        participants.ShouldBeEmpty();
    }

    [Fact]
    public void Tourist_cannot_activate_already_completed_encounter()
    {
        using var scope = Factory.Services.CreateScope();
        var touristController = CreateTouristController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

        var challengeId = -9; // Already Completed Test

        // Provera da li podaci postoje
        var challenge = dbContext.Challenges.FirstOrDefault(c => c.Id == challengeId);
        challenge.ShouldNotBeNull($"Challenge with ID {challengeId} not found in database");

        var socialEncounter = dbContext.SocialEncounters.FirstOrDefault(se => se.ChallengeId == challengeId);
        socialEncounter.ShouldNotBeNull($"SocialEncounter for Challenge {challengeId} not found in database");

        var activateRequest = new ActivateSocialEncounterRequestDto
        {
            CurrentLatitude = 45.2671,
            CurrentLongitude = 19.8453
        };

        var result = ((ObjectResult)touristController.ActivateSocialEncounter(challengeId, activateRequest).Result)?.Value
            as ActivateSocialEncounterResponseDto;

        result.ShouldNotBeNull();
        result.Success.ShouldBeFalse();
        result.Message.ShouldContain("already completed");
    }

    // =========================
    // Helper metode
    // =========================
    private static SocialEncounterController CreateTouristController(IServiceScope scope)
    {
        return new SocialEncounterController(scope.ServiceProvider.GetRequiredService<ISocialEncounterService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }*/
}