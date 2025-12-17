using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[ApiController]
[Route("api/tourist/public-entities")]
public class PublicEntityController : ControllerBase
{
    private readonly IPublicEntityService _service;

    public PublicEntityController(IPublicEntityService service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult<PublicEntityDto> GetAllPublic()
    {
        return Ok(_service.GetAllPublic());
    }

    [HttpGet("search")]
    public ActionResult<PublicEntityDto> Search(
        [FromQuery] double? minLon, 
        [FromQuery] double? minLat,
        [FromQuery] double? maxLon, 
        [FromQuery] double? maxLat,
        [FromQuery] string? query,
        [FromQuery] PublicEntityTypeDto? entityType,
        [FromQuery] FacilityType? facilityType)
    {
        return Ok(_service.SearchEntities(minLon, minLat, maxLon, maxLat, query, entityType, facilityType));
    }
}

