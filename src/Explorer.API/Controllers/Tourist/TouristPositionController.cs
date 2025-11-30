using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    private long GetTouristId()
    {
        return User.PersonId();
    }

    [HttpPost]
    public ActionResult<TouristPositionDto> Create([FromBody] TouristPositionDto touristPositionDto)
    {
        var touristId = GetTouristId();

        if (touristId != touristPositionDto.TouristId)
        {
            return Forbid();
        }

        var result = _touristPositionService.CreateOrUpdate(touristPositionDto);
        return CreatedAtAction(nameof(Create), result);
    }
}