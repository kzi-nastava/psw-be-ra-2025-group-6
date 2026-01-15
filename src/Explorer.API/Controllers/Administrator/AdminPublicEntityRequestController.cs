using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Explorer.Stakeholders.Infrastructure.Authentication;

namespace Explorer.API.Controllers.Administrator;

[Authorize(Policy = "administratorPolicy")]
[Route("api/admin/public-requests")]
[ApiController]
public class AdminPublicEntityRequestController : ControllerBase
{
    private readonly IPublicEntityRequestService _requestService;

    public AdminPublicEntityRequestController(IPublicEntityRequestService requestService)
    {
        _requestService = requestService;
    }

    private long GetAdminId() => User.PersonId();

    [HttpGet("pending")]
    public ActionResult<List<PublicEntityRequestDto>> GetPending()
    {
        var result = _requestService.GetPending();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public ActionResult<PublicEntityRequestDto> Get(long id)
    {
        var result = _requestService.Get(id);
        return Ok(result);
    }

    [HttpPost("{id}/approve")]
    public ActionResult<PublicEntityRequestDto> Approve(long id)
    {
        var adminId = GetAdminId();
        var result = _requestService.ApproveRequest(id, adminId);
        return Ok(result);
    }

    [HttpPost("{id}/reject")]
    public ActionResult<PublicEntityRequestDto> Reject(long id, [FromBody] RejectRequestDto dto)
    {
        var adminId = GetAdminId();
        var result = _requestService.RejectRequest(id, adminId, dto.Comment);
        return Ok(result);
    }
}
