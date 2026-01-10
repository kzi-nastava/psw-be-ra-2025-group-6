using Explorer.API.Controllers.Administrator;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class AdminPublicEntityRequestCommandTests : BaseToursIntegrationTest
{
    public AdminPublicEntityRequestCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Admin_Approves_PublicEntityRequest_For_KeyPoint()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var adminController = CreateAdminController(scope);
        var authorController = CreateAuthorController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();

        // Create tour first (with AuthorId from BuildContext)
        var tour = new TourDto
        {
            Name = "Test Tour for Approval",
            Description = "Test tour",
            Difficulty = TourDifficultyDto.EASY,
            Tags = new List<string> { "test" },
            Price = 0,
            AuthorId = -11  // Author from existing test data
        };
        var createdTour = tourService.Create(tour);

        // Add KeyPoint to tour
        var keyPoint = new KeyPointDto
        {
            Name = "Test KeyPoint",
            Description = "Test description",
            Longitude = 19.84,
            Latitude = 45.25,
            ImagePath = "/test.jpg",
            Secret = "Test secret"
        };
        var tourWithKeyPoint = tourService.AddKeyPoint(createdTour.Id, keyPoint);
        var keyPointId = tourWithKeyPoint.KeyPoints.First().Id;

        // Create request as author
        var requestDto = new CreatePublicEntityRequestDto
        {
            EntityType = PublicEntityTypeDto.KeyPoint,
            EntityId = keyPointId
        };
        var createdRequest = ((ObjectResult)authorController.CreateRequest(requestDto).Result)?.Value as PublicEntityRequestDto;

        // Act - Admin approves the request
        var result = ((ObjectResult)adminController.Approve(createdRequest.Id).Result)?.Value as PublicEntityRequestDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(createdRequest.Id);
        result.Status.ShouldBe(RequestStatusDto.Approved);
        result.ProcessedAt.ShouldNotBeNull();
        result.ProcessedByAdminId.ShouldBe(-1);

        // Assert - Database
        var storedRequest = dbContext.PublicEntityRequests.FirstOrDefault(r => r.Id == result.Id);
        storedRequest.ShouldNotBeNull();
        storedRequest.Status.ShouldBe(RequestStatus.Approved);
        storedRequest.ProcessedAt.ShouldNotBeNull();
        storedRequest.ProcessedByAdminId.ShouldBe(-1);
    }

    [Fact]
    public void Admin_Rejects_PublicEntityRequest_For_KeyPoint_With_Comment()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var adminController = CreateAdminController(scope);
        var authorController = CreateAuthorController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();

        // Create tour and keypoint
        var tour = tourService.Create(new TourDto
        {
            Name = "Test Tour for Rejection",
            Description = "Test",
            Difficulty = TourDifficultyDto.EASY,
            Tags = new List<string> { "test" },
            Price = 0,
            AuthorId = -11
        });

        var tourWithKeyPoint = tourService.AddKeyPoint(tour.Id, new KeyPointDto
        {
            Name = "Test KeyPoint 2",
            Description = "Test",
            Longitude = 19.84,
            Latitude = 45.25,
            ImagePath = "/test.jpg",
            Secret = "Secret"
        });
        var keyPointId = tourWithKeyPoint.KeyPoints.First().Id;

        // Create request
        var createdRequest = ((ObjectResult)authorController.CreateRequest(new CreatePublicEntityRequestDto
        {
            EntityType = PublicEntityTypeDto.KeyPoint,
            EntityId = keyPointId
        }).Result)?.Value as PublicEntityRequestDto;

        var rejectDto = new RejectRequestDto
        {
            Comment = "KeyPoint does not meet public criteria"
        };

        // Act
        var result = ((ObjectResult)adminController.Reject(createdRequest.Id, rejectDto).Result)?.Value as PublicEntityRequestDto;

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe(RequestStatusDto.Rejected);
        result.AdminComment.ShouldBe("KeyPoint does not meet public criteria");

        var storedRequest = dbContext.PublicEntityRequests.FirstOrDefault(r => r.Id == result.Id);
        storedRequest.ShouldNotBeNull();
        storedRequest.Status.ShouldBe(RequestStatus.Rejected);
    }

    [Fact]
    public void Admin_GetPending_Returns_Only_Pending_Requests()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var adminController = CreateAdminController(scope);
        var authorController = CreateAuthorController(scope);
        var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();

        // Create tour with two keypoints
        var tour = tourService.Create(new TourDto
        {
            Name = "Test Tour Pending",
            Description = "Test",
            Difficulty = TourDifficultyDto.EASY,
            Tags = new List<string> { "test" },
            Price = 0,
            AuthorId = -11
        });

        var tour1 = tourService.AddKeyPoint(tour.Id, new KeyPointDto
        {
            Name = "KP1",
            Description = "Test",
            Longitude = 19.84,
            Latitude = 45.25,
            ImagePath = "/test.jpg",
            Secret = "Secret"
        });
        var kp1Id = tour1.KeyPoints.First().Id;

        var tour2 = tourService.AddKeyPoint(tour.Id, new KeyPointDto
        {
            Name = "KP2",
            Description = "Test",
            Longitude = 19.85,
            Latitude = 45.26,
            ImagePath = "/test2.jpg",
            Secret = "Secret2"
        });
        var kp2Id = tour2.KeyPoints.Last().Id;

        // Create requests
        authorController.CreateRequest(new CreatePublicEntityRequestDto
        {
            EntityType = PublicEntityTypeDto.KeyPoint,
            EntityId = kp1Id
        });

        var request2 = ((ObjectResult)authorController.CreateRequest(new CreatePublicEntityRequestDto
        {
            EntityType = PublicEntityTypeDto.KeyPoint,
            EntityId = kp2Id
        }).Result)?.Value as PublicEntityRequestDto;

        // Approve one
        adminController.Approve(request2.Id);

        // Act
        var result = ((ObjectResult)adminController.GetPending().Result)?.Value as List<PublicEntityRequestDto>;

        // Assert
        result.ShouldNotBeNull();
        result.ShouldAllBe(r => r.Status == RequestStatusDto.Pending);
        result.ShouldContain(r => r.EntityId == kp1Id);
        result.ShouldNotContain(r => r.EntityId == kp2Id);
    }

    [Fact]
    public void Admin_Get_Returns_Specific_Request()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var adminController = CreateAdminController(scope);
        var authorController = CreateAuthorController(scope);
        var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();

        var tour = tourService.Create(new TourDto
        {
            Name = "Get Test Tour",
            Description = "Test",
            Difficulty = TourDifficultyDto.EASY,
            Tags = new List<string> { "test" },
            Price = 0,
            AuthorId = -11
        });

        var tourWithKP = tourService.AddKeyPoint(tour.Id, new KeyPointDto
        {
            Name = "Get Test KP",
            Description = "Test",
            Longitude = 19.84,
            Latitude = 45.25,
            ImagePath = "/test.jpg",
            Secret = "Secret"
        });
        var kpId = tourWithKP.KeyPoints.First().Id;

        var createdRequest = ((ObjectResult)authorController.CreateRequest(new CreatePublicEntityRequestDto
        {
            EntityType = PublicEntityTypeDto.KeyPoint,
            EntityId = kpId
        }).Result)?.Value as PublicEntityRequestDto;

        // Act
        var result = ((ObjectResult)adminController.Get(createdRequest.Id).Result)?.Value as PublicEntityRequestDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(createdRequest.Id);
        result.EntityType.ShouldBe(PublicEntityTypeDto.KeyPoint);
        result.EntityId.ShouldBe(kpId);
    }

    [Fact]
    public void Approve_Fails_When_Request_Already_Processed()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var adminController = CreateAdminController(scope);
        var authorController = CreateAuthorController(scope);
        var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();

        var tour = tourService.Create(new TourDto
        {
            Name = "Double Approve Test",
            Description = "Test",
            Difficulty = TourDifficultyDto.EASY,
            Tags = new List<string> { "test" },
            Price = 0,
            AuthorId = -11
        });

        var tourWithKP = tourService.AddKeyPoint(tour.Id, new KeyPointDto
        {
            Name = "Double KP",
            Description = "Test",
            Longitude = 19.84,
            Latitude = 45.25,
            ImagePath = "/test.jpg",
            Secret = "Secret"
        });
        var kpId = tourWithKP.KeyPoints.First().Id;

        var createdRequest = ((ObjectResult)authorController.CreateRequest(new CreatePublicEntityRequestDto
        {
            EntityType = PublicEntityTypeDto.KeyPoint,
            EntityId = kpId
        }).Result)?.Value as PublicEntityRequestDto;

        adminController.Approve(createdRequest.Id);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() =>
            adminController.Approve(createdRequest.Id)
        );
    }

    [Fact]
    public void Reject_Fails_When_Request_Already_Processed()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var adminController = CreateAdminController(scope);
        var authorController = CreateAuthorController(scope);
        var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();

        var tour = tourService.Create(new TourDto
        {
            Name = "Double Reject Test",
            Description = "Test",
            Difficulty = TourDifficultyDto.EASY,
            Tags = new List<string> { "test" },
            Price = 0,
            AuthorId = -11
        });

        var tourWithKP = tourService.AddKeyPoint(tour.Id, new KeyPointDto
        {
            Name = "Double Reject KP",
            Description = "Test",
            Longitude = 19.84,
            Latitude = 45.25,
            ImagePath = "/test.jpg",
            Secret = "Secret"
        });
        var kpId = tourWithKP.KeyPoints.First().Id;

        var createdRequest = ((ObjectResult)authorController.CreateRequest(new CreatePublicEntityRequestDto
        {
            EntityType = PublicEntityTypeDto.KeyPoint,
            EntityId = kpId
        }).Result)?.Value as PublicEntityRequestDto;

        adminController.Reject(createdRequest.Id, new RejectRequestDto { Comment = "First" });

        // Act & Assert
        Should.Throw<InvalidOperationException>(() =>
            adminController.Reject(createdRequest.Id, new RejectRequestDto { Comment = "Second" })
        );
    }

    [Fact]
    public void Reject_Fails_Without_Comment()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var adminController = CreateAdminController(scope);
        var authorController = CreateAuthorController(scope);
        var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();

        var tour = tourService.Create(new TourDto
        {
            Name = "No Comment Test",
            Description = "Test",
            Difficulty = TourDifficultyDto.EASY,
            Tags = new List<string> { "test" },
            Price = 0,
            AuthorId = -11
        });

        var tourWithKP = tourService.AddKeyPoint(tour.Id, new KeyPointDto
        {
            Name = "No Comment KP",
            Description = "Test",
            Longitude = 19.84,
            Latitude = 45.25,
            ImagePath = "/test.jpg",
            Secret = "Secret"
        });
        var kpId = tourWithKP.KeyPoints.First().Id;

        var createdRequest = ((ObjectResult)authorController.CreateRequest(new CreatePublicEntityRequestDto
        {
            EntityType = PublicEntityTypeDto.KeyPoint,
            EntityId = kpId
        }).Result)?.Value as PublicEntityRequestDto;

        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            adminController.Reject(createdRequest.Id, new RejectRequestDto { Comment = "" })
        );
    }

    private static AdminPublicEntityRequestController CreateAdminController(IServiceScope scope)
    {
        return new AdminPublicEntityRequestController(
            scope.ServiceProvider.GetRequiredService<IPublicEntityRequestService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }

    private static Explorer.API.Controllers.Author.Authoring.PublicEntityRequestController CreateAuthorController(IServiceScope scope)
    {
        return new Explorer.API.Controllers.Author.Authoring.PublicEntityRequestController(
            scope.ServiceProvider.GetRequiredService<IPublicEntityRequestService>())
        {
            ControllerContext = BuildContext("-11")
        };
    }
}
