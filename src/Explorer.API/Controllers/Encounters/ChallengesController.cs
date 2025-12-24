using Explorer.Encounters.API.Public;
using Explorer.Encounters.API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Explorer.API.Controllers.Encounters;

[ApiController]
[Route("api/encounters/challenges")]
public class ChallengesController : ControllerBase
{
    private readonly IChallengePublicService _publicService;
    private readonly Explorer.Encounters.Core.UseCases.IChallengeService _adminService;

    public ChallengesController(IChallengePublicService publicService, Explorer.Encounters.Core.UseCases.IChallengeService adminService)
    {
        _publicService = publicService;
        _adminService = adminService;
    }

    // Public: tourists see only active challenges
    [HttpGet]
    public ActionResult<List<ChallengeDto>> GetActive()
    {
        return Ok(_publicService.GetActive());
    }

    [HttpGet("{id:long}")]
    public ActionResult<ChallengeDto> Get(long id)
    {
        return Ok(_publicService.Get(id));
    }

    // Admin: get all challenges (including Draft/Archived)
    [HttpGet("all")]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult<List<ChallengeDto>> GetAll()
    {
        return Ok(_adminService.GetAll());
    }

    [HttpPost]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult<ChallengeDto> Create([FromBody] ChallengeDto dto)
    {
        return Ok(_adminService.Create(dto));
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult<ChallengeDto> Update(long id, [FromBody] ChallengeDto dto)
    {
        return Ok(_adminService.Update(id, dto));
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult Delete(long id)
    {
        _adminService.Delete(id);
        return Ok();
    }
}
