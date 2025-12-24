using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.UseCases;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Shouldly;
using System.Security.Claims;

namespace Explorer.Tours.Tests.Integration;

[Collection("Sequential")]
public class TourSearchTests : BaseToursIntegrationTest
{
    public TourSearchTests(ToursTestFactory factory) : base(factory) { }

    private static ClaimsPrincipal CreateUser()
    {
        var claims = new List<Claim>
        {
            new Claim("personId", "3")
        };

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuthType"));
    }

    private static Tour CreateTour(
    
    string name,
    string description,
    TourDifficulty difficulty, long authorId)
    {
        var tour = new Tour(name, description, difficulty, null,authorId);

        return tour;
    }


    private static void SetTourStatus(Tour tour, TourStatus status)
    {
        typeof(Tour)
            .GetProperty("Status")!
            .SetValue(tour, status);
    }


    // --------------------------------------------------

    [Fact]
    public async Task Search_returns_confirmed_tours_for_regular_user()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourSearchService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var confirmed = CreateTour
        (
            "Confirmed tour",
            "Visible to everyone",
            TourDifficulty.HARD,
             3
        );

        var draft = CreateTour
        (
            "Draft tour",
            "Not visible to everyone",
            TourDifficulty.HARD,
             3
        );

        SetTourStatus(confirmed, TourStatus.CONFIRMED);


        dbContext.Tours.AddRange(confirmed, draft);
        dbContext.SaveChanges();

        var result = await service.SearchAsync(
            query: "",
            user: CreateUser(),
            personId: 3,
            userRole: "User"
        );

        result.Any(r => r.Title == "Confirmed tour").ShouldBeTrue();
        result.Any(r => r.Title == "Draft tour").ShouldBeFalse();
    }

    // --------------------------------------------------

    [Fact]
    public async Task Search_returns_own_non_suspended_tours_for_author()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourSearchService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var myDraft = CreateTour
        (
            "My draft",
            "Visible to author",
            TourDifficulty.HARD,
             3
        );

        var mySuspended = CreateTour
        (
            "My suspended",
            "Invisible to author",
            TourDifficulty.HARD,
             3
        );

        var othersDraft = CreateTour
        (
            "Other draft",
            "Hidden author",
            TourDifficulty.HARD,
             99
        );

        SetTourStatus(mySuspended, TourStatus.SUSPENDED);

        dbContext.Tours.AddRange(myDraft, mySuspended, othersDraft);
        dbContext.SaveChanges();

        var result = await service.SearchAsync(
            query: "",
            user: CreateUser(),
            personId: 3,
            userRole: "Author"
        );

        result.Any(r => r.Title == "My draft").ShouldBeTrue();
        result.Any(r => r.Title == "My suspended").ShouldBeFalse();
        result.Any(r => r.Title == "Others draft").ShouldBeFalse();
    }

    // --------------------------------------------------

    [Fact]
    public async Task Search_returns_all_tours_for_admin()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourSearchService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var confirmed = CreateTour
        (
            "Confirmed",
            "Visible to author",
            TourDifficulty.HARD,
             3
        );

        var suspended = CreateTour
        (
            "Suspended",
            "Visible to author",
            TourDifficulty.HARD,
             99
        );
        SetTourStatus(suspended, TourStatus.SUSPENDED);
        SetTourStatus(confirmed, TourStatus.CONFIRMED);
        dbContext.Tours.AddRange(confirmed, suspended);
        dbContext.SaveChanges();

        var result = await service.SearchAsync(
            query: "",
            user: CreateUser(),
            personId: 1,
            userRole: "Administrator"
        );

        result.Any(r => r.Title == "Confirmed").ShouldBeTrue();
        result.Any(r => r.Title == "Suspended").ShouldBeTrue();
    }

    // --------------------------------------------------

    [Fact]
    public async Task Search_filters_by_query()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourSearchService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var tour1 = CreateTour
        (
            "Mountain hiking",
            "Visible to author",
            TourDifficulty.HARD,
             3
        );

        var tour2 = CreateTour
        (
            "City walking",
            "Visible to author",
            TourDifficulty.HARD,
             99
        );

        SetTourStatus(tour1, TourStatus.CONFIRMED);
        SetTourStatus(tour2, TourStatus.CONFIRMED);

        dbContext.Tours.AddRange(tour1, tour2);
        dbContext.SaveChanges();

        var result = await service.SearchAsync(
            query: "Mountain",
            user: CreateUser(),
            personId: 3,
            userRole: "User"
        );

        result.Any(r => r.Title == "Mountain hiking").ShouldBeTrue();
        result.Any(r => r.Title == "City walking").ShouldBeFalse();
    }

   
}

