using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class JournalCommandTests : BaseToursIntegrationTest
{
    public JournalCommandTests(ToursTestFactory factory) : base(factory) { }

    private const long TOURIST_ID = 1;

    [Fact]
    public async void Creates()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, TOURIST_ID);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var newEntity = new JournalDto
        {
            TouristId = TOURIST_ID,
            Name = "Novi Testni Dnevnik",
            Location = "Neko Mesto",
            TravelDate = DateTime.SpecifyKind(DateTime.Parse("2020-01-01T10:00:00Z"), DateTimeKind.Utc),
            Status = "Draft"
        };


        var result = (await controller.Create(newEntity)).Result.ShouldBeOfType<CreatedAtActionResult>().Value as JournalDto;
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.TouristId.ShouldBe(TOURIST_ID);

        var storedEntity = dbContext.Journals.FirstOrDefault(i => i.Id == result.Id);
        storedEntity.ShouldNotBeNull();
    }

    [Fact]
    public async void Updates()
    {

        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, TOURIST_ID);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var updatedEntity = new JournalDto
        {
            Id = -1,
            TouristId = TOURIST_ID,
            Name = "Ažuriran Dnevnik",
            Location = "Ažurirana Lokacija",
            TravelDate = DateTime.SpecifyKind(DateTime.Parse("2020-02-02T10:00:00Z"), DateTimeKind.Utc),
            Status = "Published"
        };

        var result = (await controller.Update(-1, updatedEntity)).Result.ShouldBeOfType<OkObjectResult>().Value as JournalDto;
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);

        var storedEntity = dbContext.Journals.FirstOrDefault(i => i.Id == -1);
        storedEntity.ShouldNotBeNull();
        storedEntity.Name.ShouldBe(updatedEntity.Name);
    }

    [Fact]
    public void Update_fails_not_owned()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, 3);
        var updatedEntity = new JournalDto
        {
            Id = -3,
            TouristId = 1,
            Name = "Tuđi Dnevnik pokusaj update",
            Location = "Ažurirana Lokacija",
            TravelDate = DateTime.SpecifyKind(DateTime.Parse("2020-02-02T10:00:00Z"), DateTimeKind.Utc),
            Status = "Draft"
        };


        var result = controller.Update(-3, updatedEntity).Result;
        result.Result.ShouldBeOfType<ForbidResult>();
    }


    [Fact]
    public async void Deletes()
    {

        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, TOURIST_ID);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();


        var result = await controller.Delete(-2);

        result.ShouldNotBeNull();
        result.ShouldBeOfType<NoContentResult>();

        var storedEntity = dbContext.Journals.FirstOrDefault(i => i.Id == -2);
        storedEntity.ShouldBeNull();
    }

    private static JournalController CreateController(IServiceScope scope, long touristId)
    {
        return new JournalController(scope.ServiceProvider.GetRequiredService<IJournalService>())
        {
            ControllerContext = BuildContext(touristId.ToString())
        };
    }
}