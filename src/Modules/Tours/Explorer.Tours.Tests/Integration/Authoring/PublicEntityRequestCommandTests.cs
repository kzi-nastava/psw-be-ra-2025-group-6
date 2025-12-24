using Explorer.API.Controllers.Author.Authoring;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using DtoFacilityType = Explorer.Tours.API.Dtos.FacilityType;

namespace Explorer.Tours.Tests.Integration.Authoring;

[Collection("Sequential")]
public class PublicEntityRequestCommandTests : BaseToursIntegrationTest
{
    public PublicEntityRequestCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates_PublicEntityRequest_For_Facility()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var facilityService = scope.ServiceProvider.GetRequiredService<IFacilityService>();

        // Create a facility first
        var facility = new FacilityDto
        {
            Name = "Test Public Facility",
            Latitude = 40.779437,
            Longitude = -73.963244,
            Type = DtoFacilityType.Restaurant,
            Comment = "Test facility for public request"
        };
        var createdFacility = facilityService.Create(facility);

        var requestDto = new CreatePublicEntityRequestDto
        {
            EntityType = PublicEntityTypeDto.Facility,
            EntityId = createdFacility.Id
        };

        // Act
        var result = ((ObjectResult)controller.CreateRequest(requestDto).Result)?.Value as PublicEntityRequestDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.AuthorId.ShouldBe(-11);
        result.EntityType.ShouldBe(PublicEntityTypeDto.Facility);
        result.EntityId.ShouldBe(createdFacility.Id);
        result.Status.ShouldBe(RequestStatusDto.Pending);
        result.RequestedAt.ShouldNotBe(default(DateTime));

        // Assert - Database
        var storedRequest = dbContext.PublicEntityRequests.FirstOrDefault(r => r.Id == result.Id);
        storedRequest.ShouldNotBeNull();
        storedRequest.Status.ShouldBe(RequestStatus.Pending);

        // Assert - Facility marked with request
        var updatedFacility = dbContext.Facility.FirstOrDefault(f => f.Id == createdFacility.Id);
        updatedFacility.ShouldNotBeNull();
        updatedFacility.PublicRequestId.ShouldBe(result.Id);
        updatedFacility.IsPublic.ShouldBeFalse();
    }

    [Fact]
    public void GetMyRequests_Returns_Author_Requests()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var facilityService = scope.ServiceProvider.GetRequiredService<IFacilityService>();

        // Create a facility and request
        var facility = new FacilityDto
        {
            Name = "My Request Test Facility",
            Latitude = 40.779437,
            Longitude = -73.963244,
            Type = DtoFacilityType.Parking
        };
        var createdFacility = facilityService.Create(facility);

        var requestDto = new CreatePublicEntityRequestDto
        {
            EntityType = PublicEntityTypeDto.Facility,
            EntityId = createdFacility.Id
        };
        controller.CreateRequest(requestDto);

        // Act
        var result = ((ObjectResult)controller.GetMyRequests().Result)?.Value as List<PublicEntityRequestDto>;

        // Assert
        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.ShouldContain(r => r.EntityId == createdFacility.Id && r.AuthorId == -11);
    }

    [Fact]
    public void GetPending_Returns_Only_Pending_Requests()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var facilityService = scope.ServiceProvider.GetRequiredService<IFacilityService>();

        // Create a facility and request
        var facility = new FacilityDto
        {
            Name = "Pending Request Test Facility",
            Latitude = 40.779437,
            Longitude = -73.963244,
            Type = DtoFacilityType.Toilet
        };
        var createdFacility = facilityService.Create(facility);

        var requestDto = new CreatePublicEntityRequestDto
        {
            EntityType = PublicEntityTypeDto.Facility,
            EntityId = createdFacility.Id
        };
        controller.CreateRequest(requestDto);

        // Act
        var result = ((ObjectResult)controller.GetPending().Result)?.Value as List<PublicEntityRequestDto>;

        // Assert
        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.ShouldAllBe(r => r.Status == RequestStatusDto.Pending);
    }

    [Fact]
    public void Create_PublicEntityRequest_Fails_When_Request_Already_Exists()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var facilityService = scope.ServiceProvider.GetRequiredService<IFacilityService>();

        // Create a facility
        var facility = new FacilityDto
        {
            Name = "Duplicate Request Test Facility",
            Latitude = 40.779437,
            Longitude = -73.963244,
            Type = DtoFacilityType.Other
        };
        var createdFacility = facilityService.Create(facility);

        var requestDto = new CreatePublicEntityRequestDto
        {
            EntityType = PublicEntityTypeDto.Facility,
            EntityId = createdFacility.Id
        };

        // Create first request
        controller.CreateRequest(requestDto);

        // Act & Assert - Try to create second request for same entity
        Should.Throw<InvalidOperationException>(() =>
            controller.CreateRequest(requestDto)
        );
    }

    [Fact]
    public void Get_Returns_Specific_Request()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var facilityService = scope.ServiceProvider.GetRequiredService<IFacilityService>();

        // Create a facility and request
        var facility = new FacilityDto
        {
            Name = "Get Request Test Facility",
            Latitude = 40.779437,
            Longitude = -73.963244,
            Type = DtoFacilityType.Restaurant
        };
        var createdFacility = facilityService.Create(facility);

        var requestDto = new CreatePublicEntityRequestDto
        {
            EntityType = PublicEntityTypeDto.Facility,
            EntityId = createdFacility.Id
        };
        var createdRequest = ((ObjectResult)controller.CreateRequest(requestDto).Result)?.Value as PublicEntityRequestDto;

        // Act
        var result = ((ObjectResult)controller.Get(createdRequest.Id).Result)?.Value as PublicEntityRequestDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(createdRequest.Id);
        result.EntityType.ShouldBe(PublicEntityTypeDto.Facility);
        result.EntityId.ShouldBe(createdFacility.Id);
    }

    [Fact]
    public void GetPaged_Returns_Paginated_Requests()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var facilityService = scope.ServiceProvider.GetRequiredService<IFacilityService>();

        // Create multiple facilities and requests
        for (int i = 0; i < 3; i++)
        {
            var facility = new FacilityDto
            {
                Name = $"Paged Test Facility {i}",
                Latitude = 40.779437,
                Longitude = -73.963244,
                Type = DtoFacilityType.Parking
            };
            var createdFacility = facilityService.Create(facility);

            var requestDto = new CreatePublicEntityRequestDto
            {
                EntityType = PublicEntityTypeDto.Facility,
                EntityId = createdFacility.Id
            };
            controller.CreateRequest(requestDto);
        }

        // Act - Use page 1 instead of 0
        var result = ((ObjectResult)controller.GetPaged(1, 2).Result)?.Value as BuildingBlocks.Core.UseCases.PagedResult<PublicEntityRequestDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.ShouldNotBeNull();
        result.Results.Count.ShouldBeLessThanOrEqualTo(2);
        result.TotalCount.ShouldBeGreaterThanOrEqualTo(3);
    }

    private static PublicEntityRequestController CreateController(IServiceScope scope)
    {
        return new PublicEntityRequestController(scope.ServiceProvider.GetRequiredService<IPublicEntityRequestService>())
        {
            ControllerContext = BuildContext("-11")
        };
    }
}
