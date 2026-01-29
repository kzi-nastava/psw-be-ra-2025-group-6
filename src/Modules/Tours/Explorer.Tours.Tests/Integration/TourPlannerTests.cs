using Explorer.API.Controllers;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration;

[Collection("Sequential")]
public class TourPlannerTests : BaseToursIntegrationTest
{
    private const long USER_ID = -1;

    public TourPlannerTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates_tour_planner_item()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, USER_ID);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var createDto = new TourPlannerCreateDto
        {
            TourId = -1,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2)
        };

        var actionResult = controller.Create(createDto);
        var okResult = actionResult.Result.ShouldBeOfType<OkObjectResult>();
        var result = okResult.Value as TourPlannerDto;

        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.UserId.ShouldBe(USER_ID);
        result.TourId.ShouldBe(createDto.TourId);

        var stored = dbContext.Set<TourPlanner>().FirstOrDefault(p => p.Id == result.Id);
        stored.ShouldNotBeNull();
        stored.UserId.ShouldBe(USER_ID);
    }

    [Fact]
    public void Gets_all_for_current_user()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, USER_ID);
        var otherController = CreateController(scope, -2);

        controller.Create(new TourPlannerCreateDto
        {
            TourId = -1,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2)
        });
        controller.Create(new TourPlannerCreateDto
        {
            TourId = -2,
            StartDate = DateTime.UtcNow.AddDays(3),
            EndDate = DateTime.UtcNow.AddDays(4)
        });
        otherController.Create(new TourPlannerCreateDto
        {
            TourId = -3,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2)
        });

        var actionResult = controller.GetAllForUser();
        var okResult = actionResult.Result.ShouldBeOfType<OkObjectResult>();
        var result = okResult.Value as List<TourPlannerDto>;

        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result.All(r => r.UserId == USER_ID).ShouldBeTrue();
    }

    [Fact]
    public void Updates_tour_planner_item()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, USER_ID);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var created = ((OkObjectResult)controller.Create(new TourPlannerCreateDto
        {
            TourId = -1,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2)
        }).Result).Value as TourPlannerDto;

        var updateDto = new TourPlannerUpdateDto
        {
            StartDate = DateTime.UtcNow.AddDays(5),
            EndDate = DateTime.UtcNow.AddDays(6)
        };

        var actionResult = controller.Update(created.Id, updateDto);
        var okResult = actionResult.Result.ShouldBeOfType<OkObjectResult>();
        var result = okResult.Value as TourPlannerDto;

        result.ShouldNotBeNull();
        result.Id.ShouldBe(created.Id);
        result.StartDate.ShouldBe(updateDto.StartDate);
        result.EndDate.ShouldBe(updateDto.EndDate);

        var stored = dbContext.Set<TourPlanner>().FirstOrDefault(p => p.Id == created.Id);
        stored.ShouldNotBeNull();
        stored.StartDate.ShouldBe(updateDto.StartDate);
        stored.EndDate.ShouldBe(updateDto.EndDate);
    }

    [Fact]
    public void Update_fails_for_other_user()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, USER_ID);
        var otherController = CreateController(scope, -2);

        var created = ((OkObjectResult)controller.Create(new TourPlannerCreateDto
        {
            TourId = -1,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2)
        }).Result).Value as TourPlannerDto;

        var updateDto = new TourPlannerUpdateDto
        {
            StartDate = DateTime.UtcNow.AddDays(3),
            EndDate = DateTime.UtcNow.AddDays(4)
        };

        var result = otherController.Update(created.Id, updateDto);
        result.Result.ShouldBeOfType<ForbidResult>();
    }

    [Fact]
    public void Deletes_tour_planner_item()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, USER_ID);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var created = ((OkObjectResult)controller.Create(new TourPlannerCreateDto
        {
            TourId = -1,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2)
        }).Result).Value as TourPlannerDto;

        var result = controller.Delete(created.Id);
        result.ShouldBeOfType<NoContentResult>();

        var stored = dbContext.Set<TourPlanner>().FirstOrDefault(p => p.Id == created.Id);
        stored.ShouldBeNull();
    }

    private static TourPlannerController CreateController(IServiceScope scope, long userId)
    {
        return new TourPlannerController(scope.ServiceProvider.GetRequiredService<ITourPlannerService>())
        {
            ControllerContext = BuildContext(userId.ToString())
        };
    }
}
