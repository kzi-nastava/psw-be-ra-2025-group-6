using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Public.TourProblem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourProblemQueryTests : BaseToursIntegrationTest
{
    public TourProblemQueryTests(ToursTestFactory factory) : base(factory) { }

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