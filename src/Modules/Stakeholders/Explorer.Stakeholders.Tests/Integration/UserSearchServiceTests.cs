using Explorer.Stakeholders.API.Services;
using Explorer.Stakeholders.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using Shouldly;
using System.Security.Claims;

namespace Explorer.Stakeholders.Tests.Integration;

[Collection("Sequential")]
public class UserSearchServiceTests : BaseStakeholdersIntegrationTest
{
    public UserSearchServiceTests(StakeholdersTestFactory factory) : base(factory) { }

    private static ClaimsPrincipal CreateUser(long personId)
    {
        var claims = new List<Claim>
        {
            new Claim("personId", personId.ToString())
        };

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuthType"));
    }
 

    [Fact]
    public async Task Search_filters_by_query()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IUserSearchService>();

        var result = await service.SearchAsync(
            query: "profileuser1",
            user: CreateUser(-23),
            personId: -23,
            userRole: "Tourist"
        );

        result.Any(r => r.Title == "profileuser1@gmail.com").ShouldBeTrue();
        result.Any(r => r.Title == "profileuser2@gmail.com").ShouldBeFalse();
    }

}
