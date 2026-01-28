using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers;

[ApiController]
[Route("api/achievements")]
public class AchievementController : ControllerBase
{
    private readonly IAchievementService _service;

    public AchievementController(IAchievementService service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult<IEnumerable<AchievementDto>> GetAll()
    {
        return Ok(_service.GetAll());
    }

    [HttpGet("{id:long}")]
    public ActionResult<AchievementDto> GetById(long id)
    {
        return Ok(_service.GetById(id));
    }
}

