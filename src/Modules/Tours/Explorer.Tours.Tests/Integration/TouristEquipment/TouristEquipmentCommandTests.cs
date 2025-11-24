using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.TouristEquipment;

[Collection("Sequential")]
public class TouristEquipmentCommandTests : BaseToursIntegrationTest
{
    public TouristEquipmentCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Adds()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var newEntity = new TouristEquipmentDto
        {
            EquipmentId = -100,
        };

        var result = ((ObjectResult)controller.Add(newEntity).Result)?.Value as TouristEquipmentDto;

        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.EquipmentId.ShouldBe(newEntity.EquipmentId);

        var stored = dbContext.TouristEquipment.FirstOrDefault(i => i.EquipmentId == newEntity.EquipmentId && i.PersonId == -1);
        stored.ShouldNotBeNull();
        stored.Id.ShouldBe(result.Id);
    }

    [Fact]
    public void Remove_existing()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var result = (OkResult)controller.Remove(-2);

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        var stored = dbContext.TouristEquipment.FirstOrDefault(i => i.Id == -2);
        stored.ShouldBeNull();
    }

    [Fact]
    public void Remove_fails_invalid()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        Should.Throw<NotFoundException>(() => controller.Remove(-1000));
    }

    private static TouristEquipmentController CreateController(IServiceScope scope)
    {
        return new TouristEquipmentController(scope.ServiceProvider.GetRequiredService<ITouristEquipmentService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}
