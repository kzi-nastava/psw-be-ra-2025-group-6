using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers;

[Authorize(Policy = "registeredUserPolicy")]
[Route("api/profile/posts")]
[ApiController]
public class ProfilePostsController : ControllerBase
{
    private readonly IProfilePostService _service;

    public ProfilePostsController(IProfilePostService service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult<PagedResult<ProfilePostDto>> GetMine([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var authorId = User.PersonId();
        var result = _service.GetPagedByAuthor(authorId, page, pageSize);
        return Ok(result);
    }

    [HttpPost]
    public ActionResult<ProfilePostDto> Create([FromBody] ProfilePostDto dto)
    {
        dto.AuthorId = User.PersonId();
        var created = _service.Create(dto);
        return CreatedAtAction(nameof(GetMine), new { id = created.Id }, created);
    }

    [HttpPut("{id:long}")]
    public ActionResult<ProfilePostDto> Update(long id, [FromBody] ProfilePostDto dto)
    {
        dto.Id = id;
        dto.AuthorId = User.PersonId();
        var updated = _service.Update(dto);
        return Ok(updated);
    }

    [HttpDelete("{id:long}")]
    public IActionResult Delete(long id)
    {
        var authorId = User.PersonId();
        _service.Delete(id, authorId);
        return NoContent();
    }
}
