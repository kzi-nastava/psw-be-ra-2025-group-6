using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers
{
    [Authorize(Policy = "registeredUserPolicy")]
    [Route("api/profile")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;

        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        [HttpGet]
        public ActionResult<UserProfileDto> Get()
        {
            var userIdClaim = User.FindFirstValue("id");
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return BadRequest("User ID not found in token.");
            }
            var userId = long.Parse(userIdClaim);
            var result = _userProfileService.Get(userId);
            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public ActionResult<UserProfileDto> GetOthersProfile(long id)
        {
            var result = _userProfileService.Get(id);
            return Ok(result);
        }

        [HttpPut]
        public ActionResult<UserProfileDto> Update([FromBody] UserProfileDto profile)
        {
            var userIdClaim = User.FindFirstValue("id");
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return BadRequest("User ID not found in token.");
            }
            profile.UserId = long.Parse(userIdClaim);
            var result = _userProfileService.Update(profile);
            return Ok(result);
        }
    }
}
