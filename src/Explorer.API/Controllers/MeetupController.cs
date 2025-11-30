using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers;

[Authorize(Policy = "meetupPolicy")]
[Route("api/tours/meetup")]
[ApiController]
public class MeetupController : ControllerBase
{
    private readonly IMeetupService _meetupService;

    public MeetupController(IMeetupService meetupService)
    {
        _meetupService = meetupService;
    }

    private long GetCreatorId()
    {
        return User.PersonId();
    }

    [HttpGet]
    public ActionResult<PagedResult<MeetupDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
    {
        return Ok(_meetupService.GetPaged(page, pageSize));
    }

    [HttpGet("{id:long}")]
    public ActionResult<MeetupDto> Get(long id)
    {
        return Ok(_meetupService.Get(id));
    }

    [HttpPost]
    public ActionResult<MeetupDto> Create([FromBody] MeetupDto meetup)
    {
        meetup.CreatorId = GetCreatorId();
        var result = _meetupService.Create(meetup);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public ActionResult<MeetupDto> Update(long id, [FromBody] MeetupDto meetup)
    {
        meetup.Id = id;
        var existingMeetup = _meetupService.Get(id);
        if (existingMeetup.CreatorId != GetCreatorId())
        {
            return Forbid();
        }
        meetup.CreatorId = GetCreatorId();
        var result = _meetupService.Update(meetup);
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        var existingMeetup = _meetupService.Get(id);
        if (existingMeetup.CreatorId != GetCreatorId())
        {
            return Forbid();
        }
        _meetupService.Delete(id);
        return NoContent();
    }
}