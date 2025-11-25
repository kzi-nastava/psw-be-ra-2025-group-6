using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Threading.Tasks;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class JournalQueryTests : BaseToursIntegrationTest
{
    public JournalQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public async void Retrieves_all_for_tourist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, 1); // Turista ID 1

        // Act
        // POPRAVKA: Koristimo .Result.ShouldBeOfType<ObjectResult>()
        var result = (await controller.GetAllForTourist()).Result.ShouldBeOfType<OkObjectResult>().Value as List<JournalDto>;
        // Assert
        result.ShouldNotBeNull();
        // Očekujemo 3 dnevnika, jer smo sve prebacili na TouristId 1 u SQL-u
        result.Count.ShouldBe(3);
    }


    [Fact]
    public void Retrieve_fails_not_owned()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, 2); // Turista ID 2

        // Act & Assert
        // Turista ID 2 pokušava da pristupi Dnevniku ID -1 (koji pripada Turisti ID 1)
        var result = controller.GetById(-1).Result;
        result.Result.ShouldBeOfType<ForbidResult>();
    }


    private static JournalController CreateController(IServiceScope scope, long touristId)
    {
        return new JournalController(scope.ServiceProvider.GetRequiredService<IJournalService>())
        {
            ControllerContext = BuildContext(touristId.ToString())
        };
    }
}