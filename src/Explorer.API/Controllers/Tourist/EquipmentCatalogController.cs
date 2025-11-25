using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/equipment/catalog")]
[ApiController]
public class EquipmentCatalogController : ControllerBase
{
    private readonly IEquipmentService _equipmentService;

    public EquipmentCatalogController(IEquipmentService equipmentService)
    {
        _equipmentService = equipmentService;
    }

    [HttpGet]
    public ActionResult<PagedResult<EquipmentDto>> GetAll([FromQuery] int page = 0, [FromQuery] int pageSize = 0)
    {
        var result = _equipmentService.GetPaged(page, pageSize);
        return Ok(result);
    }
}
