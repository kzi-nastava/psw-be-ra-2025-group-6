using Explorer.API.Controllers.Administrator.Administration;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class FacilityCommandTests : BaseToursIntegrationTest
{
    public FacilityCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var newEntity = new FacilityDto
        {
            Name = "Museum Parking Lot",
            Latitude = 40.779437,
            Longitude = -73.963244,
            Type = FacilityType.Parking,
            Comment = "Close to the main entrance of the museum."
        };

        // Act
        var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as FacilityDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(newEntity.Name);
        result.Latitude.ShouldBe(newEntity.Latitude);
        result.Longitude.ShouldBe(newEntity.Longitude);
        result.Type.ShouldBe(newEntity.Type);

        // Assert - Database
        var storedEntity = dbContext.Facility.FirstOrDefault(i => i.Id == result.Id);
        storedEntity.ShouldNotBeNull();
        storedEntity.Id.ShouldBe(result.Id);
    }

    [Fact]
    public void Create_fails_invalid_name()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedEntity = new FacilityDto
        {
            Name = "",
            Latitude = 40.779437,
            Longitude = -73.963244,
            Type = FacilityType.Toilet
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(updatedEntity));
    }

    [Fact]
    public void Create_fails_invalid_coordinates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var invalidLatitude = new FacilityDto
        {
            Name = "Invalid Facility",
            Latitude = 95.0, // Invalid latitude
            Longitude = -73.963244,
            Type = FacilityType.Toilet
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(invalidLatitude));
    }



    [Fact]
    public void Updates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var updatedEntity = new FacilityDto
        {
            Id = 1,
            Name = "Updated Central Park WC",
            Latitude = 40.785091,
            Longitude = -73.968285,
            Type = FacilityType.Toilet,
            Comment = "Recently renovated"
        };

        // Act
        var result = ((ObjectResult)controller.Update(updatedEntity).Result)?.Value as FacilityDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(1);
        result.Name.ShouldBe(updatedEntity.Name);

        // Assert - Database
        var storedEntity = dbContext.Facility.FirstOrDefault(i => i.Id == 1);
        storedEntity.ShouldNotBeNull();
        storedEntity.Name.ShouldBe(updatedEntity.Name);
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedEntity = new FacilityDto
        {
            Id = -1000,
            Name = "Non-existent Facility",
            Latitude = 40.779437,
            Longitude = -73.963244,
            Type = FacilityType.Other
        };

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Update(updatedEntity));
    }

    [Fact]
    public void Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act
        var result = (OkResult)controller.Delete(3);

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        var storedEntity = dbContext.Facility.FirstOrDefault(i => i.Id == 3);
        storedEntity.ShouldBeNull();
    }

    [Fact]
    public void Delete_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Delete(-1000));
    }

    private static FacilityController CreateController(IServiceScope scope)
    {
        return new FacilityController(scope.ServiceProvider.GetRequiredService<IFacilityService>())
        {
            ControllerContext = BuildContext("1")
        };
    }
}