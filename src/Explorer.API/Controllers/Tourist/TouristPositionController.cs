using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/position")]
public class TouristPositionController : ControllerBase
{
    private readonly ITouristPositionService _touristPositionService;

    public TouristPositionController(ITouristPositionService touristPositionService)
    {
        _touristPositionService = touristPositionService;
    }

    [HttpPost]
    public ActionResult<TouristPositionDto> Create([FromBody] TouristPositionDto touristPositionDto)
    {
        var userIdClaim = User.FindFirstValue("id");
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return BadRequest("User ID not found in token.");
        }
        var touristId = long.Parse(userIdClaim);

        if (touristId != touristPositionDto.TouristId)
        {
            return Forbid();
        }

        var result = _touristPositionService.CreateOrUpdate(touristPositionDto);
        return CreatedAtAction(nameof(Create), result);
    }
}
