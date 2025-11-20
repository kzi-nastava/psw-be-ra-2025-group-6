using System.IdentityModel.Tokens.Jwt;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.API.Controllers;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Stakeholders.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Tests.Integration.Authentication;

[Collection("Sequential")]
public class RegistrationTests : BaseStakeholdersIntegrationTest
{
    public RegistrationTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Successfully_registers_tourist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        ResetDatabase(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateController(scope);
        var account = new AccountRegistrationDto
        {
            Username = "turistaA@gmail.com",
            Email = "turistaA@gmail.com",
            Password = "turistaA",
            Name = "Žika",
            Surname = "Žikić"
        };

        // Act
        var authenticationResponse = ((ObjectResult)controller.RegisterTourist(account).Result).Value as AuthenticationTokensDto;

        // Assert - Response
        authenticationResponse.ShouldNotBeNull();
        authenticationResponse.Id.ShouldNotBe(0);
        var decodedAccessToken = new JwtSecurityTokenHandler().ReadJwtToken(authenticationResponse.AccessToken);
        var personId = decodedAccessToken.Claims.FirstOrDefault(c => c.Type == "personId");
        personId.ShouldNotBeNull();
        personId.Value.ShouldNotBe("0");

        // Assert - Database
        dbContext.ChangeTracker.Clear();
        var storedAccount = dbContext.Users.FirstOrDefault(u => u.Username == account.Email);
        storedAccount.ShouldNotBeNull();
        storedAccount.Role.ShouldBe(UserRole.Tourist);
        var storedPerson = dbContext.People.FirstOrDefault(i => i.Email == account.Email);
        storedPerson.ShouldNotBeNull();
        storedPerson.UserId.ShouldBe(storedAccount.Id);
        storedPerson.Name.ShouldBe(account.Name);
    }

    private static AuthenticationController CreateController(IServiceScope scope)
    {
        return new AuthenticationController(scope.ServiceProvider.GetRequiredService<IAuthenticationService>());
    }

    private static void ResetDatabase(IServiceScope scope)
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var path = Path.Combine(".", "..", "..", "..", "TestData");
        var scriptFiles = Directory.GetFiles(path);
        Array.Sort(scriptFiles);
        var script = string.Join('\n', scriptFiles.Select(File.ReadAllText));
        dbContext.Database.ExecuteSqlRaw(script);
    }
}