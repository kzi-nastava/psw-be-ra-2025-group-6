using Explorer.API.Controllers.Author.Authoring;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Authoring;

[Collection("Sequential")]
public class TourPublishNotificationFailureTests : IClassFixture<FailingNotificationsToursTestFactory>
{
    private readonly FailingNotificationsToursTestFactory _factory;

    public TourPublishNotificationFailureTests(FailingNotificationsToursTestFactory factory)
    {
        _factory = factory;
        using var scope = _factory.Services.CreateScope();
        ReseedAll(scope);
    }

    [Fact]
    public void Publishing_succeeds_even_when_notification_generation_fails()
    {
        using var scope = _factory.Services.CreateScope();
        var authorId = 3;
        var tourId = -1;

        var controller = new TourController(scope.ServiceProvider.GetRequiredService<ITourService>())
        {
            ControllerContext = BuildAuthorContext(authorId.ToString())
        };

        var tourDto = ((Microsoft.AspNetCore.Mvc.ObjectResult)controller.Get(tourId).Result)?.Value as TourDto;
        tourDto.Description = "Valid description for publish.";
        tourDto.Tags = new List<string> { "city", "river" };
        controller.Update(tourId, tourDto);

        var kp1 = new KeyPointDto { Name = "KP1", Description = "D1", Latitude = 45.1, Longitude = 19.1, ImagePath = "img1.jpg", Secret = "S1" };
        var kp2 = new KeyPointDto { Name = "KP2", Description = "D2", Latitude = 45.2, Longitude = 19.2, ImagePath = "img2.jpg", Secret = "S2" };
        controller.AddKeyPoint(tourId, kp1);
        controller.AddKeyPoint(tourId, kp2);

        var result = ((Microsoft.AspNetCore.Mvc.ObjectResult)controller.Publish(tourId).Result)?.Value as TourDto;
        result.ShouldNotBeNull();
        result.Status.ShouldBe(TourStatusDto.CONFIRMED);
        result.PublishedTime.ShouldNotBeNull();
    }

    private static void ReseedAll(IServiceScope scope)
    {
        var toursContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var paymentsContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
        var stakeholdersContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        ReseedPayments(paymentsContext);
        ReseedStakeholders(stakeholdersContext);
        ReseedTours(toursContext);
    }

    private static void ReseedTours(ToursContext context)
    {
        context.Database.ExecuteSqlRaw("DROP SCHEMA IF EXISTS tours CASCADE;");
        context.Database.ExecuteSqlRaw("CREATE SCHEMA IF NOT EXISTS tours;");
        context.Database.EnsureCreated();
        try
        {
            var databaseCreator = context.Database.GetService<IRelationalDatabaseCreator>();
            databaseCreator.CreateTables();
        }
        catch
        {
        }

        var scriptFolder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "TestData"));
        var scriptFiles = Directory.GetFiles(scriptFolder);
        Array.Sort(scriptFiles);
        var script = string.Join('\n', scriptFiles.Select(File.ReadAllText));
        context.Database.ExecuteSqlRaw(script);
    }

    private static void ReseedPayments(PaymentsContext context)
    {
        context.Database.ExecuteSqlRaw("CREATE SCHEMA IF NOT EXISTS payments;");
        context.Database.EnsureCreated();
        try
        {
            var databaseCreator = context.Database.GetService<IRelationalDatabaseCreator>();
            databaseCreator.CreateTables();
        }
        catch
        {
        }
    }

    private static void ReseedStakeholders(StakeholdersContext context)
    {
        context.Database.ExecuteSqlRaw("DROP SCHEMA IF EXISTS stakeholders CASCADE;");
        context.Database.ExecuteSqlRaw("CREATE SCHEMA IF NOT EXISTS stakeholders;");
        context.Database.EnsureCreated();
        try
        {
            var databaseCreator = context.Database.GetService<IRelationalDatabaseCreator>();
            databaseCreator.CreateTables();
        }
        catch
        {
        }

        var scriptFolder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "Stakeholders", "Explorer.Stakeholders.Tests", "TestData"));
        var scriptFiles = Directory.GetFiles(scriptFolder);
        Array.Sort(scriptFiles);
        var script = string.Join('\n', scriptFiles.Select(File.ReadAllText));
        context.Database.ExecuteSqlRaw(script);
    }

    private static ControllerContext BuildAuthorContext(string id)
    {
        return new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("personId", id),
                    new Claim(ClaimTypes.Role, "Author")
                }))
            }
        };
    }
}
