using Explorer.API.Contracts;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers;

[Route("api/users")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost]
    public ActionResult<AuthenticationTokensDto> RegisterTourist([FromBody] AccountRegistrationDto account)
    {
        return Ok(_authenticationService.RegisterTourist(account));
    }

    [HttpPost("login")]
    public ActionResult<AuthenticationTokensDto> Login([FromBody] CredentialsDto credentials)
    {
        var tokens = _authenticationService.Login(credentials);
        if (tokens == null)
        {
            return Unauthorized(ApiErrorFactory.Create(
                HttpContext,
                ApiErrorCodes.AuthRequired,
                "Invalid credentials.",
                "Check your username/password or create/seed a user before logging in."));
        }

        return Ok(tokens);
    }
}
