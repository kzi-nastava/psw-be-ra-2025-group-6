using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author.Authoring;

[Authorize(Policy = "authorPolicy")]
[Route("api/authoring/public-entity-requests")]
[ApiController]
public class PublicEntityRequestController : ControllerBase
{
    private readonly IPublicEntityRequestService _requestService;

    public PublicEntityRequestController(IPublicEntityRequestService requestService)
    {
        _requestService = requestService;
    }

    [HttpGet("paged")]
    public ActionResult<PagedResult<PublicEntityRequestDto>> GetPaged([FromQuery] int page, [FromQuery] int pageSize)
    {
        return Ok(_requestService.GetPaged(page, pageSize));
    }

    [HttpGet("{id:long}")]
    public ActionResult<PublicEntityRequestDto> Get(long id)
    {
        return Ok(_requestService.Get(id));
    }

    [HttpPost]
    public ActionResult<PublicEntityRequestDto> CreateRequest([FromBody] CreatePublicEntityRequestDto dto)
    {
        var authorId = User.PersonId();
        var result = _requestService.CreateRequest(dto, authorId);
        return Ok(result);
    }

    [HttpGet("my-requests")]
    public ActionResult<List<PublicEntityRequestDto>> GetMyRequests()
    {
        var authorId = User.PersonId();
        return Ok(_requestService.GetByAuthor(authorId));
    }

    [HttpGet("pending")]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult<List<PublicEntityRequestDto>> GetPending()
    {
        return Ok(_requestService.GetPending());
    }
}
