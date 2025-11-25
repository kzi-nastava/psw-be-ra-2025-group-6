using Explorer.BuildingBlocks.Core.UseCases;
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
        return Ok(_meetupService.Create(meetup));
    }

    [HttpPut("{id:long}")]
    public ActionResult<MeetupDto> Update(long id, [FromBody] MeetupDto meetup)
    {
        meetup.Id = id;
        return Ok(_meetupService.Update(meetup));
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        _meetupService.Delete(id);
        return Ok();
    }
}
