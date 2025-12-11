using Explorer.API.Controllers.Author.Authoring;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Authoring;

[Collection("Sequential")]
public class TourCommandTests : BaseToursIntegrationTest
{
    public TourCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var newEntity = new TourDto
        {
            Name = "Tura Italije",
            Description = "Obiđi gradove Italije",
            Difficulty = 0,
            Tags = new List<string> { "Evropa", "Italija" }
        };

        // Act
        var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as TourDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(newEntity.Name);
        result.Tags.ShouldBe(newEntity.Tags);
        result.Description.ShouldBe(newEntity.Description);
        result.Difficulty.ShouldBe(newEntity.Difficulty);
        result.Status.ShouldBe(TourStatusDto.DRAFT);
        result.Price.ShouldBe(0);

        // Assert - Database
        var storedEntity = dbContext.Tours.FirstOrDefault(i => i.Name == newEntity.Name);
        storedEntity.ShouldNotBeNull();
        storedEntity.Id.ShouldBe(result.Id);
        storedEntity.Name.ShouldBe(newEntity.Name);
        storedEntity.Tags.ShouldBe(newEntity.Tags);
        storedEntity.Description.ShouldBe(newEntity.Description);
        ((int)storedEntity.Difficulty).ShouldBe((int)newEntity.Difficulty);
        ((int)storedEntity.Status).ShouldBe((int)TourStatusDto.DRAFT);

        storedEntity.Price.ShouldBe(0);
    }

    [Fact]
    public void Create_fails_invalid_data()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var createdEntity = new TourDto
        {
            Description = "Test"
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(createdEntity));
    }

    [Fact]
    public void Updates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var updatedEntity = new TourDto
        {
            Id = -1,
            Name = "Tura Amerike",
            Description = "Obiđi sve Amerike",
            Difficulty = 0,
            Tags = new List<string> { "Amerika" }
        };


        // Act
        var result = ((ObjectResult)controller.Update(updatedEntity).Result)?.Value as TourDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Name.ShouldBe(updatedEntity.Name);
        result.Description.ShouldBe(updatedEntity.Description);
        result.Difficulty.ShouldBe(updatedEntity.Difficulty);
        result.Tags.ShouldBe(updatedEntity.Tags);


        // Assert - Database
        var storedEntity = dbContext.Tours.FirstOrDefault(i => i.Name == "Tura Amerike");
        storedEntity.ShouldNotBeNull();
        storedEntity.Description.ShouldBe(updatedEntity.Description);
        storedEntity.Name.ShouldBe(updatedEntity.Name);
        storedEntity.Tags.ShouldBe(updatedEntity.Tags);
        storedEntity.Description.ShouldBe(updatedEntity.Description);
        ((int)storedEntity.Difficulty).ShouldBe((int)updatedEntity.Difficulty);
        var oldEntity = dbContext.Equipment.FirstOrDefault(i => i.Name == "Tura Londona");
        oldEntity.ShouldBeNull();
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedEntity = new TourDto
        {
            Id = -1000,
            Name = "Tura Amerike",
            Description = "Obiđi sve Amerike",
            Difficulty = 0,
            Tags = new List<string> { "Amerika" }
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
        var result = (OkResult)controller.Delete(-2);

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        var storedCourse = dbContext.Tours.FirstOrDefault(i => i.Id == -2);
        storedCourse.ShouldBeNull();
    }

    [Fact]
    public void Delete_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => controller.Delete(-3));
    }

    [Fact]
    public void Delete_fails_confirmed_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Delete(-1000));
    }

    private static TourController CreateController(IServiceScope scope)
    {
        return new TourController(scope.ServiceProvider.GetRequiredService<ITourService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}
