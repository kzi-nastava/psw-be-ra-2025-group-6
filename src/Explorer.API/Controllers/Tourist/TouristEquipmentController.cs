using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/tourist-equipment")]
[ApiController]
public class TouristEquipmentController : ControllerBase
{
    private readonly ITouristEquipmentService _touristEquipmentService;

    public TouristEquipmentController(ITouristEquipmentService touristEquipmentService)
    {
        _touristEquipmentService = touristEquipmentService;
    }

    [HttpGet]
    public ActionResult<PagedResult<TouristEquipmentDto>> GetOwned([FromQuery] int page, [FromQuery] int pageSize)
    {
        var result = _touristEquipmentService.GetOwned(User.PersonId(), page, pageSize);
        return Ok(result);
    }

    [HttpPost]
    public ActionResult<TouristEquipmentDto> Add([FromBody] TouristEquipmentDto dto)
    {
        dto.PersonId = User.PersonId();
        var result = _touristEquipmentService.Add(dto);
        return Ok(result);
    }

    [HttpDelete("{equipmentId:long}")]
    public ActionResult Remove(long equipmentId)
    {
        _touristEquipmentService.Remove(User.PersonId(), equipmentId);
        return Ok();
    }
}
