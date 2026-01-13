using Explorer.API.Controllers;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.IdentityModel.Tokens.Jwt;

namespace Explorer.Stakeholders.Tests.Integration.Authentication;

[Collection("Sequential")]
public class LoginTests : BaseStakeholdersIntegrationTest
{
    public LoginTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Successfully_logs_in()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var loginSubmission = new CredentialsDto { Username = "turista1@gmail.com", Password = "turista1" };

        // Act
        var authenticationResponse = ((ObjectResult)controller.Login(loginSubmission).Result).Value as AuthenticationTokensDto;

        // Assert
        authenticationResponse.ShouldNotBeNull();
        authenticationResponse.Id.ShouldBe(-21);
        var decodedAccessToken = new JwtSecurityTokenHandler().ReadJwtToken(authenticationResponse.AccessToken);
        var personId = decodedAccessToken.Claims.FirstOrDefault(c => c.Type == "personId");
        personId.ShouldNotBeNull();
        personId.Value.ShouldBe("-21");
    }

    [Fact]
    public void Not_registered_user_fails_login()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var loginSubmission = new CredentialsDto { Username = "turistaY@gmail.com", Password = "turista1" };

        // Act & Assert
        Should.Throw<UnauthorizedAccessException>(() => controller.Login(loginSubmission));
    }

    [Fact]
    public void Invalid_password_fails_login()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var loginSubmission = new CredentialsDto { Username = "turista3@gmail.com", Password = "123" };

        // Act & Assert
        Should.Throw<UnauthorizedAccessException>(() => controller.Login(loginSubmission));
    }

    private static AuthenticationController CreateController(IServiceScope scope)
    {
        return new AuthenticationController(
            scope.ServiceProvider.GetRequiredService<IAuthenticationService>(),
            scope.ServiceProvider.GetRequiredService<Explorer.Payments.API.Public.IWalletService>());
    }
}
