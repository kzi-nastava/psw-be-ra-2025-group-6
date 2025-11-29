using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.Tours.API.Public;
using Explorer.Tours.Infrastructure.Database;
using Explorer.API.Controllers;

namespace Explorer.Tours.Tests.Integration;

[Collection("Sequential")]
public class MeetupTests : BaseToursIntegrationTest
{
    public MeetupTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Get_all()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetAll(0, 0).Result)?.Value as PagedResult<MeetupDto>;

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
        var newEntity = CreateDto(0, "New Meetup", "New Description", DateTime.UtcNow.AddDays(5), 45.3, 19.9, -21, DateTime.UtcNow);

        // Act
        var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as MeetupDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(newEntity.Name);
        result.CreatorId.ShouldBe(newEntity.CreatorId);

        // Assert - Database
        var storedEntity = dbContext.Meetups.FirstOrDefault(m => m.Id == result.Id);
        storedEntity.ShouldNotBeNull();
        storedEntity.Id.ShouldBe(result.Id);
    }

    [Fact]
    public void Create_fails_invalid_data()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var newEntity = CreateDto(0, "", "New Description", DateTime.UtcNow.AddDays(5), 45.3, 19.9, -21, DateTime.UtcNow); // Invalid name

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(newEntity));
    }

    [Fact]
    public void Updates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var updatedEntity = CreateDto(-1, "Updated Meetup Name", "Description 1", new DateTime(2025, 11, 20, 10, 0, 0, DateTimeKind.Utc), 45.25, 19.83, -21, new DateTime(2025, 11, 19, 10, 0, 0, DateTimeKind.Utc));

        // Act
        var result = ((ObjectResult)controller.Update(updatedEntity.Id, updatedEntity).Result)?.Value as MeetupDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(updatedEntity.Id);
        result.Name.ShouldBe(updatedEntity.Name);

        // Assert - Database
        var storedEntity = dbContext.Meetups.FirstOrDefault(m => m.Id == updatedEntity.Id);
        storedEntity.ShouldNotBeNull();
        storedEntity.Name.ShouldBe(updatedEntity.Name);
    }

    [Fact]
    public void Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        long meetupIdToDelete = -3;

        // Act
        var result = (OkResult)controller.Delete(meetupIdToDelete);

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        var storedEntity = dbContext.Meetups.FirstOrDefault(m => m.Id == meetupIdToDelete);
        storedEntity.ShouldBeNull();
    }

    private static MeetupDto CreateDto(long id, string name, string description, DateTime eventDate, double latitude, double longitude, long creatorId, DateTime lastModified)
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

    private static MeetupController CreateController(IServiceScope scope)
    {
        return new MeetupController(scope.ServiceProvider.GetRequiredService<IMeetupService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}
