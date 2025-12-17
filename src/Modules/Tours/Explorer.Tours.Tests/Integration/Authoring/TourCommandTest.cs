using Explorer.API.Controllers.Author.Authoring;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

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

        var claims = new List<Claim>
{
        new Claim("personId", "3")
};
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var user = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };


        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var newEntity = new TourDto
        {
            Name = "Tura Italije",
            Description = "Obiđi gradove Italije",
            Difficulty=0,
            Tags=new List<string> { "Evropa", "Italija" },
            Duration= new List<TourDurationDto>
            {
                new TourDurationDto
                {
                    TravelType=TravelTypeDto.CAR,
                    Minutes=15
                },
                
            }
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
        result.AuthorId.ShouldBe(newEntity.AuthorId);

        // Assert - Database
        var storedEntity = dbContext.Tours.FirstOrDefault(i => i.Name == newEntity.Name);
        storedEntity.ShouldNotBeNull();
        storedEntity.Id.ShouldBe(result.Id);
        storedEntity.Name.ShouldBe(newEntity.Name);
        storedEntity.Tags.ShouldBe(newEntity.Tags);
        storedEntity.Description.ShouldBe(newEntity.Description);
        ((int)storedEntity.Difficulty).ShouldBe((int)newEntity.Difficulty);
        ((int)storedEntity.Status).ShouldBe((int)TourStatusDto.DRAFT);
        storedEntity.AuthorId.ShouldBe(newEntity.AuthorId);

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
        var createdEntity1 = new TourDto
        {
            Name = "Tura Italije",
            Description = "Obiđi gradove Italije",
            Difficulty = 0,
            Tags = new List<string> { "Evropa", "Italija" }
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

        var claims = new List<Claim>
{
        new Claim("personId", "3")
};
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var user = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };



        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var updatedEntity = new TourDto
        {
            Name = "Tura Amerike",
            Description = "Obiđi sve Amerike",
            Difficulty = 0,
            Tags = new List<string> { "Amerika" },
            Price=1000,
            Status = TourStatusDto.DRAFT,
            AuthorId=3
        };


        // Act
        var result = ((ObjectResult)controller.Update(-1,updatedEntity).Result)?.Value as TourDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Name.ShouldBe(updatedEntity.Name);
        result.Description.ShouldBe(updatedEntity.Description);
        result.Difficulty.ShouldBe(updatedEntity.Difficulty);
        result.Tags.ShouldBe(updatedEntity.Tags);
        result.Price.ShouldBe(updatedEntity.Price);
        result.Status.ShouldBe(updatedEntity.Status);
        result.AuthorId.ShouldBe(3);
        

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
        storedEntity.Price.ShouldBe(updatedEntity.Price);
        ((int)storedEntity.Status).ShouldBe((int)updatedEntity.Status);
        storedEntity.AuthorId.ShouldBe(3);
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
        Should.Throw<NotFoundException>(() => controller.Update(updatedEntity.Id,updatedEntity));
    }


    [Fact]
    public void Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var claims = new List<Claim>
{
        new Claim("personId", "3") 
};
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var user = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };


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

        var claims = new List<Claim>
{
        new Claim("personId", "3")
};
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var user = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Delete(-1000));
    }


    [Fact]
    public void Delete_fails_confirmed_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var claims = new List<Claim>
{
        new Claim("personId", "4")
};
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var user = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => controller.Delete(-3));
    }

    [Fact]
    public void Delete_fails_invalid_AuthorId()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var claims = new List<Claim>
{
        new Claim("personId", "2")
};
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var user = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        // Act & Assert
        Should.Throw<ForbiddenException>(() => controller.Delete(-3));
    }

    private static TourController CreateController(IServiceScope scope)
    {
        return new TourController(scope.ServiceProvider.GetRequiredService<ITourService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}
