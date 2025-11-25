using Explorer.API.Controllers.Tourist;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;

namespace Explorer.Stakeholders.Tests.Integration;

[Collection("Sequential")]
public class TourProblemQueryTests : BaseStakeholdersIntegrationTest
{
    public TourProblemQueryTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public async void Retrieves_all_by_tourist()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var actionResult = await controller.GetMyProblems();
        var result = (actionResult.Result as OkObjectResult)?.Value as List<TourProblemDto>;

        result.ShouldNotBeNull();
        result.Count.ShouldBe(3);
    }

    private static TourProblemController CreateController(IServiceScope scope)
    {
        return new TourProblemController(scope.ServiceProvider.GetRequiredService<ITourProblemService>())
        {
            ControllerContext = BuildContext("1")
        };
    }
}