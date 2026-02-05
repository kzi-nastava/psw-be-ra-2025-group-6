using Explorer.Stakeholders.API.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace Explorer.Stakeholders.Tests.Integration;

[Collection("Sequential")]
public class TouristPreferencesIntegrationTests : BaseStakeholdersIntegrationTest
{
    public TouristPreferencesIntegrationTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public async Task Tourist_can_upsert_and_get_preferences()
    {
        using var client = CreateClient();
        await Authenticate(client, "turista1@gmail.com", "turista1");

        var upsert = new TouristPreferencesUpsertDto
        {
            PreferredDifficulty = 1,
            WalkRating = 3,
            BikeRating = 1,
            CarRating = 2,
            BoatRating = 0,
            Tags = new List<string> { "city", "river" }
        };

        var upsertResponse = await client.PutAsJsonAsync("/api/tourist/preferences/me", upsert);
        upsertResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var getResponse = await client.GetAsync("/api/tourist/preferences/me");
        getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await getResponse.Content.ReadFromJsonAsync<TouristPreferencesDto>();
        result.ShouldNotBeNull();
        result.TouristId.ShouldBe(-21);
        result.PreferredDifficulty.ShouldBe(1);
        result.WalkRating.ShouldBe(3);
        result.Tags.ShouldContain("city");
    }

    [Fact]
    public async Task Non_tourist_cannot_access_preferences()
    {
        using var client = CreateClient();
        await Authenticate(client, "autor1@gmail.com", "autor1");

        var response = await client.GetAsync("/api/tourist/preferences/me");
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    private async Task Authenticate(HttpClient client, string username, string password)
    {
        var loginDto = new CredentialsDto { Username = username, Password = password };
        var response = await client.PostAsJsonAsync("/api/users/login", loginDto);
        var authTokens = await response.Content.ReadFromJsonAsync<AuthenticationTokensDto>();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authTokens.AccessToken);
    }

    private HttpClient CreateClient()
    {
        return Factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }
}
