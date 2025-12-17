using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.Stakeholders.API.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Policy = "AdministratorPolicy")]
    public class AdminUsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public AdminUsersController(IUserService userService)
        {
            _userService = userService;
        }

        // POST: api/admin/users
        [HttpPost]
        public ActionResult<UserDto> CreateUser([FromBody] CreateUserDto dto)
        {
            var user = _userService.CreateUser(dto);
            return Ok(user);
        }

        // GET: api/admin/users
        [HttpGet]
        public ActionResult<IEnumerable<UserDto>> GetAllUsers()
        {
            var users = _userService.GetAllUsers();
            return Ok(users);
        }

        // PUT: api/admin/users/{userId}/block
        [HttpPut("{userId}/block")]
        public IActionResult BlockUser(long userId)
        {
            _userService.BlockUser(userId);
            return NoContent();
        }

    }
}
