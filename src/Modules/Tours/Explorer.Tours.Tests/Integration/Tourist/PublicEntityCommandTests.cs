using Explorer.API.Controllers.Author.Authoring;
using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class PublicEntityCommandTests : BaseToursIntegrationTest
{
    public PublicEntityCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Search_returns_only_public_entities_in_bounds()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var action = controller.Search(20, 45, 21, 46, null, null, null);
        var result = (action.Result as OkObjectResult)?.Value as PublicEntityDto;

        result.ShouldNotBeNull();

        result.Facilities.ShouldContain(f => f.Name == "Second Facility");
        result.Facilities.ShouldNotContain(f => f.Name == "Test Facility");

        result.KeyPoints.ShouldNotBeNull();
    }

    [Fact]
    public void Search_without_bounds_returns_all_public_facilities()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var action = controller.Search(
            minLon: null,
            minLat: null,
            maxLon: null,
            maxLat: null,
            query: null,
            entityType: PublicEntityTypeDto.Facility,
            facilityType: null);

        var result = (action.Result as OkObjectResult)?.Value as PublicEntityDto;

        result.ShouldNotBeNull();

        result.Facilities.ShouldContain(f => f.Name == "Central Park WC");
        result.Facilities.ShouldContain(f => f.Name == "Second Facility");

        result.Facilities.ShouldNotContain(f => f.Name == "Test Facility");

        result.KeyPoints.ShouldBeEmpty();
    }

    [Fact]
    public void Search_filters_public_facilities_by_text_and_facility_type()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var action = controller.Search(
            minLon: null,
            minLat: null,
            maxLon: null,
            maxLat: null,
            query: "Second",
            entityType: PublicEntityTypeDto.Facility,
            facilityType: FacilityType.Restaurant);

        var result = (action.Result as OkObjectResult)?.Value as PublicEntityDto;

        result.ShouldNotBeNull();

        result.Facilities.Count.ShouldBe(1);
        result.Facilities[0].Name.ShouldBe("Second Facility");
        result.Facilities[0].Type.ShouldBe(FacilityType.Restaurant);

        result.KeyPoints.ShouldBeEmpty();
    }

    private static PublicEntityController CreateController(IServiceScope scope)
    {
        return new PublicEntityController(scope.ServiceProvider.GetRequiredService<IPublicEntityService>())
        {
            ControllerContext = BuildContext("-11")
        };
    }

}

