using Explorer.API.Controllers.Author.Authoring;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Linq;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Authoring;

[Collection("Sequential")]
public class TourArchiveIntegrationTests : BaseToursIntegrationTest
{
    private const long AUTHOR_ID = 3;
    private const long OTHER_AUTHOR_ID = 4;

    public TourArchiveIntegrationTests(ToursTestFactory factory) : base(factory) { }

    // ============================================
    // ARHIVIRANJE - POZITIVNI TESTOVI
    // ============================================

    [Fact]
    public void Archive_PublishedTour_Succeeds()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, OTHER_AUTHOR_ID);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Resetujemo turu -3 na CONFIRMED ako je prethodni test promenio
        var tourToReset = dbContext.Tours.First(t => t.Id == -3);
        if (tourToReset.Status == TourStatus.ARCHIVED)
        {
            controller.Activate(-3);
        }

        // Tura -3 je CONFIRMED (Status = 1) i pripada autoru 4
        var tourBeforeArchive = dbContext.Tours.First(t => t.Id == -3);
        tourBeforeArchive.Status.ShouldBe(TourStatus.CONFIRMED);

        // Act
        var result = controller.Archive(-3);

        // Assert
        result.Result.ShouldBeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();

        var archivedTour = okResult.Value as TourDto;
        archivedTour.ShouldNotBeNull();
        archivedTour.Status.ShouldBe(TourStatusDto.ARCHIVED);

        // Provera u bazi
        var dbTour = dbContext.Tours.First(t => t.Id == -3);
        dbTour.Status.ShouldBe(TourStatus.ARCHIVED);

        // Cleanup - vrati u CONFIRMED za ostale testove
        controller.Activate(-3);
    }

    // ============================================
    // ARHIVIRANJE - NEGATIVNI TESTOVI (STATUS)
    // ============================================

    [Fact]
    public void Archive_DraftTour_Throws()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, AUTHOR_ID);

        // Ture -1 i -2 su u statusu DRAFT (Status = 0)

        // Act & Assert
        Should.Throw<InvalidOperationException>(() =>
        {
            controller.Archive(-1);
        }).Message.ShouldContain("Tour must be published to be archived");

        Should.Throw<InvalidOperationException>(() =>
        {
            controller.Archive(-2);
        }).Message.ShouldContain("Tour must be published to be archived");
    }

    [Fact]
    public void Archive_NonPublishedTour_Throws()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, AUTHOR_ID);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Tura -1 je u statusu DRAFT (0)
        var tour = dbContext.Tours.First(t => t.Id == -1);
        tour.Status.ShouldBe(TourStatus.DRAFT);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() =>
        {
            controller.Archive(-1);
        });
    }

    [Fact]
    public void Archive_AlreadyArchivedTour_Throws()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, OTHER_AUTHOR_ID);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Resetujemo turu ako treba
        var tourToCheck = dbContext.Tours.First(t => t.Id == -3);
        if (tourToCheck.Status == TourStatus.ARCHIVED)
        {
            controller.Activate(-3);
        }

        // Prvo arhiviramo turu
        controller.Archive(-3);

        var archivedTour = dbContext.Tours.First(t => t.Id == -3);
        archivedTour.Status.ShouldBe(TourStatus.ARCHIVED);

        // Act & Assert - pokušaj ponovnog arhiviranja
        Should.Throw<InvalidOperationException>(() =>
        {
            controller.Archive(-3);
        }).Message.ShouldContain("Tour must be published to be archived");

        // Cleanup
        controller.Activate(-3);
    }

    // ============================================
    // ARHIVIRANJE - NEGATIVNI TESTOVI (OWNERSHIP)
    // ============================================

    [Fact]
    public void Archive_TourByDifferentAuthor_Throws()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, AUTHOR_ID); // Autor 3
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Resetujemo turu ako treba
        var tourToCheck = dbContext.Tours.First(t => t.Id == -3);
        if (tourToCheck.Status == TourStatus.ARCHIVED)
        {
            var ownerController = CreateController(scope, OTHER_AUTHOR_ID);
            ownerController.Activate(-3);
        }

        // Tura -3 pripada autoru 4, ne autoru 3
        var tour = dbContext.Tours.First(t => t.Id == -3);
        tour.AuthorId.ShouldBe(OTHER_AUTHOR_ID);
        tour.Status.ShouldBe(TourStatus.CONFIRMED);

        // Act & Assert
        Should.Throw<ForbiddenException>(() =>
        {
            controller.Archive(-3);
        }).Message.ShouldContain("Only the owner can archive the tour");
    }

    // ============================================
    // AKTIVACIJA - POZITIVNI TESTOVI
    // ============================================

    [Fact]
    public void Activate_ArchivedTour_ReturnsToPublished()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, OTHER_AUTHOR_ID);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Resetujemo turu ako treba
        var tourToCheck = dbContext.Tours.First(t => t.Id == -3);
        if (tourToCheck.Status == TourStatus.ARCHIVED)
        {
            controller.Activate(-3);
        }

        // Prvo arhiviramo turu -3 kroz controller (da validacija prođe)
        controller.Archive(-3);

        // Verifikujemo da je arhivirana u bazi
        var archivedTour = dbContext.Tours.First(t => t.Id == -3);
        archivedTour.Status.ShouldBe(TourStatus.ARCHIVED);

        // Act - reaktiviramo turu
        var result = controller.Activate(-3);

        // Assert
        result.Result.ShouldBeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();

        var activatedTour = okResult.Value as TourDto;
        activatedTour.ShouldNotBeNull();
        activatedTour.Status.ShouldBe(TourStatusDto.CONFIRMED);

        // Provera u bazi
        var dbTour = dbContext.Tours.First(t => t.Id == -3);
        dbTour.Status.ShouldBe(TourStatus.CONFIRMED);
    }

    [Fact]
    public void Archive_ThenActivate_FullCycle_Succeeds()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, OTHER_AUTHOR_ID);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var tourId = -3;

        // Resetujemo turu ako treba
        var tourToCheck = dbContext.Tours.First(t => t.Id == tourId);
        if (tourToCheck.Status == TourStatus.ARCHIVED)
        {
            controller.Activate(tourId);
        }

        // Početno stanje: CONFIRMED
        var initialTour = dbContext.Tours.First(t => t.Id == tourId);
        initialTour.Status.ShouldBe(TourStatus.CONFIRMED);

        // Act 1: Arhiviranje
        var archiveResult = controller.Archive(tourId);
        archiveResult.Result.ShouldBeOfType<OkObjectResult>();

        var archivedTour = dbContext.Tours.First(t => t.Id == tourId);
        archivedTour.Status.ShouldBe(TourStatus.ARCHIVED);

        // Act 2: Reaktivacija
        var activateResult = controller.Activate(tourId);
        activateResult.Result.ShouldBeOfType<OkObjectResult>();

        // Assert: Vraćena u CONFIRMED
        var finalTour = dbContext.Tours.First(t => t.Id == tourId);
        finalTour.Status.ShouldBe(TourStatus.CONFIRMED);
    }

    [Fact]
    public void Archive_ThenActivate_MultipleTimes_Succeeds()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, OTHER_AUTHOR_ID);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var tourId = -3;

        // Resetujemo turu ako treba
        var tourToCheck = dbContext.Tours.First(t => t.Id == tourId);
        if (tourToCheck.Status == TourStatus.ARCHIVED)
        {
            controller.Activate(tourId);
        }

        // Act & Assert - Ciklus 1
        controller.Archive(tourId);
        dbContext.Tours.First(t => t.Id == tourId).Status.ShouldBe(TourStatus.ARCHIVED);

        controller.Activate(tourId);
        dbContext.Tours.First(t => t.Id == tourId).Status.ShouldBe(TourStatus.CONFIRMED);

        // Act & Assert - Ciklus 2
        controller.Archive(tourId);
        dbContext.Tours.First(t => t.Id == tourId).Status.ShouldBe(TourStatus.ARCHIVED);

        controller.Activate(tourId);
        dbContext.Tours.First(t => t.Id == tourId).Status.ShouldBe(TourStatus.CONFIRMED);
    }

    // ============================================
    // AKTIVACIJA - NEGATIVNI TESTOVI (STATUS)
    // ============================================

    [Fact]
    public void Activate_DraftTour_Throws()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, AUTHOR_ID);

        // Pokušaj aktivacije ture koja nije arhivirana (DRAFT)
        // Act & Assert
        Should.Throw<InvalidOperationException>(() =>
        {
            controller.Activate(-1);
        });
    }

    [Fact]
    public void Activate_PublishedTour_Throws()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, OTHER_AUTHOR_ID);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Resetujemo turu ako je arhivirana
        var tourToCheck = dbContext.Tours.First(t => t.Id == -3);
        if (tourToCheck.Status == TourStatus.ARCHIVED)
        {
            controller.Activate(-3);
        }

        // Tura -3 je već CONFIRMED
        var tour = dbContext.Tours.First(t => t.Id == -3);
        tour.Status.ShouldBe(TourStatus.CONFIRMED);

        // Act & Assert - ne može aktivirati već aktivnu turu
        Should.Throw<InvalidOperationException>(() =>
        {
            controller.Activate(-3);
        });
    }

    // ============================================
    // AKTIVACIJA - NEGATIVNI TESTOVI (OWNERSHIP)
    // ============================================

    [Fact]
    public void Activate_TourByDifferentAuthor_Throws()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Prvo vlasnik arhivira svoju turu
        var controllerOwner = CreateController(scope, OTHER_AUTHOR_ID);
        controllerOwner.Archive(-3);

        // Verifikujemo da je arhivirana
        var tour = dbContext.Tours.First(t => t.Id == -3);
        tour.Status.ShouldBe(TourStatus.ARCHIVED);

        // Sada pokušava drugi autor da je aktivira
        var controllerOther = CreateController(scope, AUTHOR_ID); // Autor 3 pokušava

        // Act & Assert
        Should.Throw<ForbiddenException>(() =>
        {
            controllerOther.Activate(-3);
        }).Message.ShouldContain("Only the owner can activate the tour");
    }

    // ============================================
    // EDGE CASES
    // ============================================

    [Fact]
    public void Archive_NonExistentTour_Throws()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, AUTHOR_ID);

        // Act & Assert - nepostojeći ID
        Should.Throw<NotFoundException>(() =>
        {
            controller.Archive(-999);
        });
    }

    [Fact]
    public void Activate_NonExistentTour_Throws()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, AUTHOR_ID);

        // Act & Assert - nepostojeći ID
        Should.Throw<NotFoundException>(() =>
        {
            controller.Activate(-999);
        });
    }

    private static TourController CreateController(IServiceScope scope, long authorId)
    {
        return new TourController(scope.ServiceProvider.GetRequiredService<ITourService>())
        {
            ControllerContext = BuildContext(authorId.ToString())
        };
    }
}