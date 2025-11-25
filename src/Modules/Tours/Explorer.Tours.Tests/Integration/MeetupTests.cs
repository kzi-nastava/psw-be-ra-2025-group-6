using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Tests;
using Explorer.Tours.API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text;
using System.Text.Json;
using Explorer.Tours.Core.Domain;
using Shouldly;
using Explorer.Stakeholders.API.Dtos;
using System.Net.Http.Headers;

namespace Explorer.Tours.Tests.Integration;

[Collection("Sequential")]
public class MeetupTests : BaseToursIntegrationTest
{
    public MeetupTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public async Task Get_all()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var client = Factory.CreateClient();
        await SetupAuthentication(client);

        // Act
        var response = await client.GetAsync("/api/tours/meetup");
        var stream = await response.Content.ReadAsStreamAsync();
        var pagedResult = JsonSerializer.Deserialize<TestPagedResult<MeetupDto>>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Assert
        response.EnsureSuccessStatusCode();
        pagedResult.ShouldNotBeNull();
        pagedResult.Results.Count.ShouldBe(3);
        pagedResult.TotalCount.ShouldBe(3);
    }

    [Fact]
    public async Task Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var client = Factory.CreateClient();
        await SetupAuthentication(client);
        var newEntity = CreateDto(0, "New Meetup", "New Description", DateTime.UtcNow.AddDays(5), 45.3, 19.9, -21, DateTime.UtcNow);
        var jsonContent = new StringContent(JsonSerializer.Serialize(newEntity), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/tours/meetup", jsonContent);
        var stream = await response.Content.ReadAsStreamAsync();
        var createdEntity = JsonSerializer.Deserialize<MeetupDto>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Assert
        response.EnsureSuccessStatusCode();
        createdEntity.ShouldNotBeNull();
        createdEntity.Id.ShouldNotBe(0);
        createdEntity.Name.ShouldBe(newEntity.Name);
        createdEntity.CreatorId.ShouldBe(newEntity.CreatorId);
    }

    [Fact]
    public async Task Create_fails_invalid_data()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var client = Factory.CreateClient();
        await SetupAuthentication(client);
        var newEntity = CreateDto(0, "", "New Description", DateTime.UtcNow.AddDays(5), 45.3, 19.9, -21, DateTime.UtcNow); // Invalid name
        var jsonContent = new StringContent(JsonSerializer.Serialize(newEntity), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/tours/meetup", jsonContent);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Updates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var client = Factory.CreateClient();
        await SetupAuthentication(client);
        var updatedEntity = CreateDto(-1, "Updated Meetup Name", "Description 1", new DateTime(2025, 11, 20, 10, 0, 0, DateTimeKind.Utc), 45.25, 19.83, -21, new DateTime(2025, 11, 19, 10, 0, 0, DateTimeKind.Utc));
        var jsonContent = new StringContent(JsonSerializer.Serialize(updatedEntity), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PutAsync($"/api/tours/meetup/{updatedEntity.Id}", jsonContent);
        var stream = await response.Content.ReadAsStreamAsync();
        var result = JsonSerializer.Deserialize<MeetupDto>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Assert
        response.EnsureSuccessStatusCode();
        result.ShouldNotBeNull();
        result.Id.ShouldBe(updatedEntity.Id);
        result.Name.ShouldBe(updatedEntity.Name);
    }

    [Fact]
    public async Task Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var client = Factory.CreateClient();
        await SetupAuthentication(client);
        long meetupIdToDelete = -3;

        // Act
        var response = await client.DeleteAsync($"/api/tours/meetup/{meetupIdToDelete}");

        // Assert
        response.EnsureSuccessStatusCode();

        // Optional: Verify it's really gone
        var checkResponse = await client.GetAsync($"/api/tours/meetup");
        var stream = await checkResponse.Content.ReadAsStreamAsync();
        var pagedResult = JsonSerializer.Deserialize<TestPagedResult<MeetupDto>>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        pagedResult.Results.Any(m => m.Id == meetupIdToDelete).ShouldBeFalse();
    }

    private static MeetupDto CreateDto(long id, string name, string description, DateTime eventDate, double latitude, double longitude, long creatorId, DateTime lastModified)
    {
        return new MeetupDto
        {
            Id = id,
            Name = name,
            Description = description,
            EventDate = eventDate,
            Latitude = latitude,
            Longitude = longitude,
            CreatorId = creatorId,
            LastModified = lastModified
        };
    }

    private async Task SetupAuthentication(HttpClient client)
    {
        var token = await Login(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task<string> Login(HttpClient client)
    {
        var credentials = new CredentialsDto { Username = "turista1@gmail.com", Password = "turista1" };
        var jsonContent = new StringContent(JsonSerializer.Serialize(credentials), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/users/login", jsonContent);
        var stream = await response.Content.ReadAsStreamAsync();
        var authTokens = JsonSerializer.Deserialize<AuthenticationTokensDto>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return authTokens.AccessToken;
    }

    private class TestPagedResult<T>
    {
        public List<T> Results { get; set; }
        public int TotalCount { get; set; }
    }
}
