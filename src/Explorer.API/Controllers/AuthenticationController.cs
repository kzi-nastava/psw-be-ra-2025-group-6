using Explorer.Payments.API.Public;
using Explorer.API.Contracts;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers;

[Route("api/users")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IWalletService _walletService;

    public AuthenticationController(IAuthenticationService authenticationService, IWalletService walletService)
    {
        _authenticationService = authenticationService;
        _walletService = walletService;
    }

    [HttpPost]
    public ActionResult<AuthenticationTokensDto> RegisterTourist([FromBody] AccountRegistrationDto account)
    {
        var tokens = _authenticationService.RegisterTourist(account);
        _walletService.CreateForTourist(tokens.PersonId);
        return Ok(tokens);
    }

    [HttpPost("login")]
    public ActionResult<AuthenticationTokensDto> Login([FromBody] CredentialsDto credentials)
    {
        var tokens = _authenticationService.Login(credentials);
        if (tokens == null)
        {
            var context = HttpContext ?? new DefaultHttpContext();
            return Unauthorized(ApiErrorFactory.Create(
                context,
                ApiErrorCodes.AuthRequired,
                "Invalid credentials.",
                "Check your username/password or create/seed a user before logging in."));
        }

        return Ok(tokens);
    }
}
