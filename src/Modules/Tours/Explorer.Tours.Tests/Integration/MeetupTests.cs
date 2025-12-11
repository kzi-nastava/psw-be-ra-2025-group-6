using Explorer.API.Controllers;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration;

[Collection("Sequential")]
public class MeetupTests : BaseToursIntegrationTest
{
    private const long CREATOR_ID = -21;

    public MeetupTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Get_all()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var actionResult = controller.GetAll(0, 10);
        var okResult = actionResult.Result.ShouldBeOfType<OkObjectResult>();
        var result = okResult.Value as PagedResult<MeetupDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.Count.ShouldBe(3);
        result.TotalCount.ShouldBe(3);
    }

    [Fact]
    public void Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var newEntity = CreateDto(0, "New Meetup", "New Description",
            DateTime.UtcNow.AddDays(5), 45.3, 19.9, CREATOR_ID, DateTime.UtcNow);

        // Act
        var actionResult = controller.Create(newEntity);
        var createdResult = actionResult.Result.ShouldBeOfType<CreatedAtActionResult>();
        var result = createdResult.Value as MeetupDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(newEntity.Name);
        result.CreatorId.ShouldBe(CREATOR_ID);
        result.Description.ShouldBe(newEntity.Description);

        // Assert - Database
        var storedEntity = dbContext.Meetups.FirstOrDefault(m => m.Id == result.Id);
        storedEntity.ShouldNotBeNull();
        storedEntity.Name.ShouldBe(newEntity.Name);
    }

    [Fact]
    public void Create_fails_invalid_data()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var newEntity = CreateDto(0, "", "New Description",
            DateTime.UtcNow.AddDays(5), 45.3, 19.9, CREATOR_ID, DateTime.UtcNow);

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(newEntity));
    }

    [Fact]
    public void Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act
        var result = controller.Delete(-3);

        // Assert - Response
        result.ShouldNotBeNull();
        result.ShouldBeOfType<NoContentResult>();

        // Assert - Database
        var storedEntity = dbContext.Meetups.FirstOrDefault(m => m.Id == -3);
        storedEntity.ShouldBeNull();
    }

    [Fact]
    public void Delete_fails_not_owned()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, -999);

        // Act
        var result = controller.Delete(-1);

        // Assert
        result.ShouldBeOfType<ForbidResult>();
    }

    private static MeetupController CreateController(IServiceScope scope, long creatorId = CREATOR_ID)
    {
        return new MeetupController(scope.ServiceProvider.GetRequiredService<IMeetupService>())
        {
            ControllerContext = BuildContext(creatorId.ToString())
        };
    }

    private static MeetupDto CreateDto(long id, string name, string description, DateTime eventDate,
        double latitude, double longitude, long creatorId, DateTime lastModified)
    {
        return new MeetupDto
        {
            Id = id,
            Name = name,
            Description = description,
            EventDate = eventDate,
            Latitude = latitude,
            Longitude = longitude,
            CreatorId = creatorId,
            LastModified = lastModified
        };
    }
}