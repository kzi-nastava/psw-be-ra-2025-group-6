using Explorer.API.Controllers.Administrator.Administration;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class AnnualAwardQueryTests : BaseToursIntegrationTest
{
    public AnnualAwardQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_all()
    {
        //Arange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        //Act
        var result = ((ObjectResult)controller.GetAll(1, 10).Result)?.Value as PagedResult<AnnualAwardDto>;

        //Assert
        result.ShouldNotBeNull();
        result.Results.Count.ShouldBe(3);
        result.TotalCount.ShouldBe(3);
    }

    private static AnnualAwardController CreateController(IServiceScope scope)
    {
        return new AnnualAwardController(scope.ServiceProvider.GetRequiredService<IAnnualAwardService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}
