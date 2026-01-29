using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.Stakeholders.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpGet ("{userId}")]
        public ActionResult<UserDto> GetUser(long userId)
        {
            var user = _userService.GetUser(userId);
            return Ok(user);
        }

    }
}
