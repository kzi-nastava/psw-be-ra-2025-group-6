using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration;

[Authorize(Policy = "administratorPolicy")]
[Route("api/administration/equipment")]
[ApiController]
public class EquipmentController : ControllerBase
{
    private readonly IEquipmentService _equipmentService;

    public EquipmentController(IEquipmentService equipmentService)
    {
        _equipmentService = equipmentService;
    }

    [HttpGet("paged")]
    public ActionResult<PagedResult<EquipmentDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
    {
        return Ok(_equipmentService.GetPaged(page, pageSize));
    }
    [HttpGet]
    public ActionResult<List<EquipmentDto>> GetAll()
    {
        return Ok(_equipmentService.GetAll());
    }

    [HttpPost]
    public ActionResult<EquipmentDto> Create([FromBody] EquipmentDto equipment)
    {
        return Ok(_equipmentService.Create(equipment));
    }

    [HttpPut("{id:long}")]
    public ActionResult<EquipmentDto> Update([FromBody] EquipmentDto equipment)
    {
        return Ok(_equipmentService.Update(equipment));
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        _equipmentService.Delete(id);
        return Ok();
    }
}