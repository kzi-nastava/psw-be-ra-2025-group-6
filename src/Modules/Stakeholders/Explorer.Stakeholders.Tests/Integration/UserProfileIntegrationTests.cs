using Explorer.Stakeholders.API.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace Explorer.Stakeholders.Tests.Integration;

[Collection("Sequential")]
public class UserProfileIntegrationTests : BaseStakeholdersIntegrationTest
{
    public UserProfileIntegrationTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public async Task Retrieves_successfully()
    {
        // Arrange
        using var client = CreateClient();
        await Authenticate(client, "profileuser1@gmail.com", "test1234");

        // Act
        var response = await client.GetAsync("/api/profile");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var profile = await response.Content.ReadFromJsonAsync<UserProfileDto>();
        profile.ShouldNotBeNull();
        profile.UserId.ShouldBe(-100);
        profile.Name.ShouldBe("Test");
        profile.Surname.ShouldBe("UserOne");
    }

    [Fact]
    public async Task Retrieves_fails_not_found()
    {
        // Arrange
        using var client = CreateClient();
        await Authenticate(client, "profilenotfound@gmail.com", "test9012");

        // Act
        var response = await client.GetAsync("/api/profile");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Updates_successfully()
    {
        // Arrange
        using var client = CreateClient();
        await Authenticate(client, "profileuser1@gmail.com", "test1234");
        var updatedProfile = new UserProfileDto
        {
            Name = "UpdatedName",
            Surname = "UpdatedSurname",
            ProfilePicture = "updated.jpg",
            Biography = "Updated bio.",
            Quote = "Updated quote.",
            IsFollowedByMe = false,
            Username = "profileuser1"//,
            //UserId = -100
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/profile", updatedProfile);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UserProfileDto>();
        result.ShouldNotBeNull();
        result.Name.ShouldBe("UpdatedName");
        result.Surname.ShouldBe("UpdatedSurname");
    }

    [Fact]
    public async Task Creates_successfully()
    {
        // Arrange
        using var client = CreateClient();
        await Authenticate(client, "profileuser2@gmail.com", "test5678");
        var newProfile = new UserProfileDto
        {
            Name = "NewName",
            Surname = "NewSurname",
            ProfilePicture = "new.jpg",
            Biography = "New bio.",
            Quote = "New quote.",
            IsFollowedByMe = false,
            //UserId = -101,
            Username = "profileuser2"
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/profile", newProfile);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UserProfileDto>();
        result.ShouldNotBeNull();
        result.UserId.ShouldBe(-101);
        result.Name.ShouldBe("NewName");
    }

    [Fact]
    public async Task Update_fails_invalid_data()
    {
        // Arrange
        using var client = CreateClient();
        await Authenticate(client, "profileuser1@gmail.com", "test1234");
        var invalidProfile = new UserProfileDto
        {
            Name = "", // Invalid name
            Surname = "UpdatedSurname",
            ProfilePicture = "updated.jpg",
            Biography = "Updated bio.",
            Quote = "Updated quote."
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/profile", invalidProfile);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
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
        var client = Factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        return client;
    }
}
