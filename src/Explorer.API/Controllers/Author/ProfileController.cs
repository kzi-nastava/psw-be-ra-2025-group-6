using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author;

[Authorize(Policy = "authorPolicy")]
[Route("api/author/profile")]
[ApiController]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet]
    public ActionResult<ProfileDto> Get()
    {
        var personId = User.PersonId();
        var result = _profileService.GetProfile(personId);
        return Ok(result);
    }

    [HttpPut]
    public ActionResult<ProfileDto> Update([FromBody] ProfileDto dto)
    {
        var personId = User.PersonId();
        var result = _profileService.UpdateProfile(personId, dto);
        return Ok(result);
    }
}