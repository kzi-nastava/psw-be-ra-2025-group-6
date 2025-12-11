using Explorer.API.Controllers.Tourist;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration;

[Collection("Sequential")]
public class TourReviewTests : BaseToursIntegrationTest
{
    public TourReviewTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates_tour_review()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var newReview = new TourReviewDto
        {
            TourId = 1,
            Rating = 5,
            Comment = "Excellent tour!",
            CompletedPercent = 100,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        // Act
        var result = ((ObjectResult)controller.Create(newReview).Result)?.Value as TourReviewDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Comment.ShouldBe(newReview.Comment);
        result.Rating.ShouldBe(newReview.Rating);

        var storedReview = dbContext.TourReviews.FirstOrDefault(r => r.Comment == newReview.Comment);
        storedReview.ShouldNotBeNull();
        storedReview.Id.ShouldBe(result.Id);
    }

    [Fact]
    public void Create_fails_invalid_data()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var invalidReview = new TourReviewDto
        {
            TourId = 1,
            Rating = 6, // Invalid rating
            Comment = "Invalid",
            CompletedPercent = 100
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(invalidReview));
    }

    [Fact]
    public void Get_my_reviews()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetMyReviews().Result)?.Value as List<TourReviewDto>;

        // Assert
        result.ShouldNotBeNull();
    }

    [Fact]
    public void Updates_tour_review()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var newReview = new TourReviewDto
        {
            TourId = 2,
            Rating = 4,
            Comment = "Good tour",
            CompletedPercent = 80,
            CreatedAt = DateTime.UtcNow
        };
        var createdReview = ((ObjectResult)controller.Create(newReview).Result)?.Value as TourReviewDto;

        var reviewToUpdate = new TourReviewDto
        {
            Id = createdReview.Id,
            TourId = 2,
            Rating = 3,
            Comment = "Updated comment",
            CompletedPercent = 85,
            CreatedAt = createdReview.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var result = ((ObjectResult)controller.Update(createdReview.Id, reviewToUpdate).Result)?.Value as TourReviewDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(createdReview.Id);
        result.Comment.ShouldBe("Updated comment");
        result.Rating.ShouldBe(3);

        var storedReview = dbContext.TourReviews.FirstOrDefault(r => r.Id == createdReview.Id);
        storedReview.ShouldNotBeNull();
        storedReview.Comment.ShouldBe("Updated comment");
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updateReview = new TourReviewDto
        {
            Id = -100,
            TourId = 1,
            Rating = 5,
            Comment = "Update",
            CompletedPercent = 100
        };

        Should.Throw<KeyNotFoundException>(() => controller.Update(-100, updateReview));
    }

    [Fact]
    public void Deletes_tour_review()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var newReview = new TourReviewDto
        {
            TourId = 3,
            Rating = 5,
            Comment = "To be deleted",
            CompletedPercent = 100,
            CreatedAt = DateTime.UtcNow
        };
        var createdReview = ((ObjectResult)controller.Create(newReview).Result)?.Value as TourReviewDto;

        // Act
        var result = (NoContentResult)controller.Delete(createdReview.Id);

        // Assert
        result.StatusCode.ShouldBe(204);

        var storedReview = dbContext.TourReviews.FirstOrDefault(r => r.Id == createdReview.Id);
        storedReview.ShouldBeNull();
    }

    private static TourReviewController CreateController(IServiceScope scope)
    {
        return new TourReviewController(scope.ServiceProvider.GetRequiredService<ITourReviewService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}
