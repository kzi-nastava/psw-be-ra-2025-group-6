using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration;

[Collection("Sequential")]
public class TourProblemCommandTests : BaseStakeholdersIntegrationTest
{
    public TourProblemCommandTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public async void Creates()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var newEntity = new TourProblemDto
        {
            TourId = 1,
            TouristId = 1,
            Category = 0,
            Priority = 1,
            Description = "Nova tehnicka greska na turi"
        };

        var actionResult = await controller.Create(newEntity);
        var result = (actionResult.Result as OkObjectResult)?.Value as TourProblemDto;

        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Description.ShouldBe(newEntity.Description);

        var storedEntity = dbContext.TourProblems.FirstOrDefault(i => i.Description == newEntity.Description);
        storedEntity.ShouldNotBeNull();
        storedEntity.Id.ShouldBe(result.Id);
    }

    [Fact]
    public async void Create_fails_invalid_data()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var newEntity = new TourProblemDto
        {
            TourId = 1,
            TouristId = 1,
            Category = 0,
            Priority = 1,
            Description = ""
        };

        await Should.ThrowAsync<ArgumentException>(async () => await controller.Create(newEntity));
    }

    [Fact]
    public async void Updates()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var updatedEntity = new TourProblemDto
        {
            Id = -1,
            TourId = 1,
            TouristId = 1,
            Category = 3,
            Priority = 2,
            Description = "Azuriran opis problema"
        };

        var actionResult = await controller.Update(-1, updatedEntity);
        var result = (actionResult.Result as OkObjectResult)?.Value as TourProblemDto;

        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Description.ShouldBe(updatedEntity.Description);

        var storedEntity = dbContext.TourProblems.FirstOrDefault(i => i.Id == -1);
        storedEntity.ShouldNotBeNull();
        storedEntity.Description.ShouldBe(updatedEntity.Description);
    }

    [Fact]
    public async void Update_fails_invalid_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedEntity = new TourProblemDto
        {
            Id = -1000,
            Description = "Test"
        };

        await Should.ThrowAsync<NotFoundException>(async () => await controller.Update(-1000, updatedEntity));
    }

    [Fact]
    public async void Deletes()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var actionResult = await controller.Delete(-3);
        var result = actionResult as NoContentResult;

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(204);

        var storedEntity = dbContext.TourProblems.FirstOrDefault(i => i.Id == -3);
        storedEntity.ShouldBeNull();
    }

    [Fact]
    public async void Delete_fails_invalid_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        await Should.ThrowAsync<NotFoundException>(async () => await controller.Delete(-1000));
    }

    private TourProblemController CreateController(IServiceScope scope)
    {
        return new TourProblemController(scope.ServiceProvider.GetRequiredService<ITourProblemService>())
        {
            ControllerContext = BuildContext("1")
        };
    }
}