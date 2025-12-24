using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Shouldly;
using Explorer.Encounters.API.Dtos;

namespace Explorer.Encounters.Tests.Integration;

[Collection("Sequential")]
public class ChallengeQueryTests : BaseEncountersIntegrationTest
{
    public ChallengeQueryTests(EncountersTestFactory factory) : base(factory) { }

    [Fact]
    public void Public_GetActive_Returns_Only_Active()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = new Explorer.API.Controllers.Encounters.ChallengesController(
            scope.ServiceProvider.GetRequiredService<Explorer.Encounters.API.Public.IChallengePublicService>(),
            scope.ServiceProvider.GetRequiredService<Explorer.Encounters.Core.UseCases.IChallengeService>());

        var result = ((ObjectResult)controller.GetActive().Result).Value as List<ChallengeDto>;

        result.ShouldNotBeNull();
        // Ensure all returned are Active
        result.All(r => r.Status == "Active").ShouldBeTrue();
    }

    [Fact]
    public void Admin_GetAll_Returns_All()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = new Explorer.API.Controllers.Encounters.ChallengesController(
            scope.ServiceProvider.GetRequiredService<Explorer.Encounters.API.Public.IChallengePublicService>(),
            scope.ServiceProvider.GetRequiredService<Explorer.Encounters.Core.UseCases.IChallengeService>());

        var result = ((ObjectResult)controller.GetAll().Result).Value as List<ChallengeDto>;

        result.ShouldNotBeNull();
        // Expect at least one draft or archived present in seeded data
        result.ShouldNotBeEmpty();
    }
}
