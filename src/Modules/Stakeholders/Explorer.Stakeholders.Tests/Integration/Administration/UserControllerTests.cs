using Explorer.Stakeholders.API.Dtos; // adjust if needed
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Explorer.Stakeholders.Tests.Integration.AdminUsers
{
    [Collection("Sequential")]
    public class UserControllerTests : IAsyncLifetime, IClassFixture<StakeholdersTestFactory>
    {
        private readonly StakeholdersTestFactory _factory;
        private HttpClient _client;

        public UserControllerTests(StakeholdersTestFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        // Runs once before tests
        public async Task InitializeAsync()
        {
            // 1) reset DB
            await ExecuteSqlScript("TestData/a-delete.sql");
            await ExecuteSqlScript("TestData/b-users.sql");
            await ExecuteSqlScript("TestData/c-people.sql");
            await ExecuteSqlScript("TestData/d-reviewapp.sql");

            // 2) authenticate as admin
            await AuthenticateClientAsAdmin();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        private string GetTestDataPath(string filename) =>
            Path.Combine(AppContext.BaseDirectory, filename.Replace('/', Path.DirectorySeparatorChar));

        private async Task ExecuteSqlScript(string relativePath)
        {
            var fullPath = GetTestDataPath(relativePath);
            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"SQL script not found: {fullPath}");

            var sql = await File.ReadAllTextAsync(fullPath);

            await using var scope = _factory.Services.CreateAsyncScope();
            var ctx = scope.ServiceProvider.GetRequiredService<Explorer.Stakeholders.Infrastructure.Database.StakeholdersContext>();
            await ctx.Database.ExecuteSqlRawAsync("CREATE SCHEMA IF NOT EXISTS stakeholders;");
            await ctx.Database.EnsureCreatedAsync();
            try
            {
                var databaseCreator = ctx.Database.GetService<IRelationalDatabaseCreator>();
                databaseCreator.CreateTables();
            }
            catch
            {
                // Tables already exist
            }
            await ctx.Database.ExecuteSqlRawAsync(sql);
        }

        private async Task AuthenticateClientAsAdmin()
        {
            var payload = new { username = "admin@gmail.com", password = "admin123" };
            var resp = await _client.PostAsJsonAsync("/api/users/login", payload);
            resp.EnsureSuccessStatusCode();

            var tokenResp = await resp.Content.ReadFromJsonAsync<LoginResponse>();
            tokenResp.ShouldNotBeNull();
            tokenResp.AccessToken.ShouldNotBeNullOrEmpty();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResp.AccessToken);
        }

        private record LoginResponse(long Id = 0, string AccessToken = "");

        // ---------------- TESTS ----------------

        [Fact]
        public async Task Admin_Can_Get_All_Users()
        {
            var resp = await _client.GetAsync("/api/admin/users");
            resp.StatusCode.ShouldBe(HttpStatusCode.OK);

            var users = await resp.Content.ReadFromJsonAsync<UserDto[]>();
            users.ShouldNotBeNull();
            users.Length.ShouldBeGreaterThan(0);
            users.Any(u => u.Username == "admin@gmail.com").ShouldBeTrue();
        }

        [Fact]
        public async Task Admin_Can_Create_User_and_Person()
        {
            // Unique username/email per run
            var unique = Guid.NewGuid().ToString("N").Substring(0, 8);
            var dto = new CreateUserDto
            {
                Username = $"testuser{unique}@example.com",
                Email = $"testuser{unique}@example.com",
                Password = "TestPass123",
                Name = "Test",
                Surname = "User",
                Role = "Author",
                IsActive = true
            };

            var resp = await _client.PostAsJsonAsync("/api/admin/users", dto);

            if (!resp.IsSuccessStatusCode)
            {
                var text = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Server error: {resp.StatusCode}, {text}");
            }

            resp.StatusCode.ShouldBe(HttpStatusCode.OK);

            var created = await resp.Content.ReadFromJsonAsync<UserDto>();
            created.ShouldNotBeNull();
            created.Username.ShouldBe(dto.Username);
        }

        [Fact]
        public async Task Admin_Can_Block_User()
        {
            // Arrange: existing user
            var userId = -21L;

            // Act
            var blockResp = await _client.PutAsync($"/api/admin/users/{userId}/block", null);

            // Assert
            blockResp.StatusCode.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);

            // verify in DB
            await using var scope = _factory.Services.CreateAsyncScope();
            var ctx = scope.ServiceProvider.GetRequiredService<Explorer.Stakeholders.Infrastructure.Database.StakeholdersContext>();
            var user = await ctx.Users.FindAsync(userId);
            user.ShouldNotBeNull();
            user.IsActive.ShouldBeFalse();
        }
    }
}
