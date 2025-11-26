using Explorer.API.Controllers.Administrator.Administration;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class MonumentQueryTests : BaseToursIntegrationTest
{
    public MonumentQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_all()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetAll(0, 0).Result)?.Value as PagedResult<MonumentDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.Count.ShouldBe(5);
        result.TotalCount.ShouldBe(5);
    }

    [Fact]
    public void Retrieves_single()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.Get(-1).Result)?.Value as MonumentDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Name.ShouldContain("Ulcinj");
    }

    [Fact]
    public void Get_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Get(-1000));
    }

    private static MonumentController CreateController(IServiceScope scope)
    {
        return new MonumentController(scope.ServiceProvider.GetRequiredService<IMonumentService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}
