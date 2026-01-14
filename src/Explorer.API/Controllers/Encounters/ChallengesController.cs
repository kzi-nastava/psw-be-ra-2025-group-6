using Explorer.Encounters.API.Public;
using Explorer.Encounters.API.Dtos; // <-- This using is required for ChallengeDto
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Encounters.Core.UseCases;

namespace Explorer.API.Controllers.Encounters;

[ApiController]
[Route("api/encounters/challenges")]
public class ChallengesController : ControllerBase
{
    private readonly IChallengePublicService _publicService;
    private readonly IChallengeService _adminService;

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

    // Admin: create challenge
    [HttpPost]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult<ChallengeDto> Create([FromBody] ChallengeDto dto)
    {
        return Ok(_adminService.Create(dto));
    }

    [HttpPost("create-by-tourist")]
    [Authorize(Policy = "touristPolicy")]
    public ActionResult<ChallengeDto> CreateByTourist([FromBody] ChallengeDto dto)
    {
        var touristId = User.PersonId();
        if (touristId == 0)
        {
            Console.WriteLine("ERROR: PersonId from token is null or 0!");
            return BadRequest(new { message = "Invalid token, cannot determine user ID." });
        }

        Console.WriteLine($" TouristId from token: {touristId}"); 

        try
        {
            var result = _adminService.CreateByTourist(dto, touristId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($" Error: {ex.Message}"); 
            return BadRequest(new { message = ex.Message });
        }
    }

    // Admin: get challenges pending approval
    [HttpGet("pending-approval")]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult<List<ChallengeDto>> GetPendingApproval()
    {
        return Ok(_adminService.GetPendingApproval());
    }

    // Admin: approve tourist-created challenge
    [HttpPut("{id:long}/approve")]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult<ChallengeDto> Approve(long id)
    {
        try
        {
            return Ok(_adminService.ApproveChallenge(id));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Admin: reject tourist-created challenge
    [HttpPut("{id:long}/reject")]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult<ChallengeDto> Reject(long id)
    {
        try
        {
            return Ok(_adminService.RejectChallenge(id));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
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
