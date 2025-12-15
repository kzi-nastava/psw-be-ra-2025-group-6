using Explorer.API.Controllers.Administrator.Administration;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Admin;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class AnnualAwardCommandTests : BaseToursIntegrationTest
{
    public AnnualAwardCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        //Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var newEntity = new AnnualAwardDto
        {
            Name = "Best Serbian tour",
            Description = "Best Serbian based tour",
            Year = 2023,
            VotingStartDate = DateTime.UtcNow,
            VotingEndDate = DateTime.UtcNow.AddDays(7)
        };

        //Act
        var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as AnnualAwardDto;

        //Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(newEntity.Name);
        result.Description.ShouldBe(newEntity.Description);
        result.Year.ShouldBe(newEntity.Year);
        result.Status.ShouldBe(AwardStatusDto.DRAFT);
        result.VotingStartDate.ShouldBe(newEntity.VotingStartDate);
        result.VotingEndDate.ShouldBe(newEntity.VotingEndDate);

        //Assert - Database
        var storedEntity = dbContext.AnnualAwards.FirstOrDefault(i => i.Name == newEntity.Name);
        storedEntity.ShouldNotBeNull();
        storedEntity.Id.ShouldBe(result.Id);
        storedEntity.Name.ShouldBe(newEntity.Name);
        storedEntity.Description.ShouldBe(newEntity.Description);
        storedEntity.Year.ShouldBe(newEntity.Year);
        ((int)storedEntity.Status).ShouldBe((int)AwardStatusDto.DRAFT);
        storedEntity.VotingStartDate.ShouldBe(newEntity.VotingStartDate);
        storedEntity.VotingEndDate.ShouldBe(newEntity.VotingEndDate);
    }

    [Fact]
    public void Create_fails_invalid_data()
    {
        //Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedEntity = new AnnualAwardDto
        {
            Description = "Test"
        };

        //Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(updatedEntity));
    }

    [Fact]
    public void Updates()
    {
        //Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var updatedEntity = new AnnualAwardDto
        {
            Id = -1,
            Name = "The greatest tourist group",
            Description = "Biggest tourist group of the year",
            Year = 2020,
            VotingStartDate = DateTime.UtcNow,
            VotingEndDate = DateTime.UtcNow.AddDays(7)
        };

        //Act
        var result = ((ObjectResult)controller.Update(updatedEntity).Result)?.Value as AnnualAwardDto;

        //Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(updatedEntity.Name);
        result.Description.ShouldBe(updatedEntity.Description);
        result.Year.ShouldBe(updatedEntity.Year);
        result.VotingStartDate.ShouldBe(updatedEntity.VotingStartDate);
        result.VotingEndDate.ShouldBe(updatedEntity.VotingEndDate);

        //Assert - Database
        var storedEntity = dbContext.AnnualAwards.FirstOrDefault(i => i.Name == "The greatest tourist group");
        storedEntity.ShouldNotBeNull();
        storedEntity.Id.ShouldBe(result.Id);
        storedEntity.Name.ShouldBe(updatedEntity.Name);
        storedEntity.Description.ShouldBe(updatedEntity.Description);
        storedEntity.Year.ShouldBe(updatedEntity.Year);
        storedEntity.VotingStartDate.ShouldBe(updatedEntity.VotingStartDate);
        storedEntity.VotingEndDate.ShouldBe(updatedEntity.VotingEndDate);

        var oldEntity = dbContext.AnnualAwards.FirstOrDefault(i => i.Name == "Best tour guide");
        oldEntity.ShouldBeNull();
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedEntity = new AnnualAwardDto
        {
            Id = -1000,
            Name = "The greatest tourist group",
            Description = "Biggest tourist group of the year",
            Year = 2020,
            VotingStartDate = DateTime.UtcNow,
            VotingEndDate = DateTime.UtcNow.AddDays(7)
        };

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Update(updatedEntity));
    }

    [Fact]
    public void Deletes()
    {
        //Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        //Act
        var result = (OkResult)controller.Delete(-2);

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        var storedCourse = dbContext.AnnualAwards.FirstOrDefault(i => i.Id == -2);
        storedCourse.ShouldBeNull();
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

    private static AnnualAwardController CreateController(IServiceScope scope)
    {
        return new AnnualAwardController(scope.ServiceProvider.GetRequiredService<IAnnualAwardService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}
