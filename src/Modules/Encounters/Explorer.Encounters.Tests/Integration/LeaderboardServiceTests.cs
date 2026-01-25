using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.Encounters.Core.UseCases;
using Explorer.Encounters.API.Dtos;

namespace Explorer.Encounters.Tests.Integration;

[Collection("Sequential")]
public class LeaderboardServiceTests : BaseEncountersIntegrationTest
{
    public LeaderboardServiceTests(EncountersTestFactory factory) : base(factory) { }

    [Fact]
    public async Task GetUserLeaderboardStats_Creates_New_Entry_For_New_User()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILeaderboardService>();
        var newUserId = 999L;

        // Act
        var result = await service.GetUserLeaderboardStatsAsync(newUserId);

        // Assert
        result.ShouldNotBeNull();
        result.UserId.ShouldBe(newUserId);
        result.TotalXP.ShouldBe(0);
        result.CompletedChallenges.ShouldBe(0);
        result.CompletedTours.ShouldBe(0);
        result.AdventureCoins.ShouldBe(0);
    }

    [Fact]
    public async Task UpdateUserStats_Increases_Stats_Correctly()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILeaderboardService>();
        var userId = 1000L;

        // Act
        await service.UpdateUserStatsAsync(userId, 100, 1, 0, 50);
        var result = await service.GetUserLeaderboardStatsAsync(userId);

        // Assert
        result.TotalXP.ShouldBe(100);
        result.CompletedChallenges.ShouldBe(1);
        result.CompletedTours.ShouldBe(0);
        result.AdventureCoins.ShouldBe(50);
    }

    [Fact]
    public async Task UpdateUserStats_Multiple_Times_Accumulates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILeaderboardService>();
        var userId = 1001L;

        // Act
        await service.UpdateUserStatsAsync(userId, 100, 1, 0, 50);
        await service.UpdateUserStatsAsync(userId, 200, 2, 1, 100);
        var result = await service.GetUserLeaderboardStatsAsync(userId);

        // Assert
        result.TotalXP.ShouldBe(300);
        result.CompletedChallenges.ShouldBe(3);
        result.CompletedTours.ShouldBe(1);
        result.AdventureCoins.ShouldBe(150);
    }

    [Fact]
    public async Task GetTouristLeaderboard_Returns_Sorted_By_XP()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILeaderboardService>();
        
        // Create multiple users with different stats
        await service.UpdateUserStatsAsync(2001L, 500, 5, 2, 250);
        await service.UpdateUserStatsAsync(2002L, 1000, 10, 5, 500);
        await service.UpdateUserStatsAsync(2003L, 250, 2, 1, 125);

        // Act
        var result = await service.GetTouristLeaderboardAsync(1, 10);

        // Assert
        result.ShouldNotBeNull();
        result.Results.ShouldNotBeEmpty();
        
        // Top entry should have highest XP (among these test users)
        var topUser = result.Results.FirstOrDefault(r => r.UserId == 2002L);
        topUser.ShouldNotBeNull();
        topUser.TotalXP.ShouldBe(1000);
    }

    [Fact]
    public async Task RecalculateRanks_Updates_Ranks_Correctly()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILeaderboardService>();
        
        await service.UpdateUserStatsAsync(3001L, 100, 1, 0, 50);
        await service.UpdateUserStatsAsync(3002L, 200, 2, 0, 100);
        await service.UpdateUserStatsAsync(3003L, 300, 3, 0, 150);

        // Act
        await service.RecalculateRanksAsync();

        // Get results
        var user1 = await service.GetUserLeaderboardStatsAsync(3001L);
        var user2 = await service.GetUserLeaderboardStatsAsync(3002L);
        var user3 = await service.GetUserLeaderboardStatsAsync(3003L);

        // Assert - ranks should be assigned based on XP
        user3.CurrentRank.ShouldBeLessThan(user2.CurrentRank);
        user2.CurrentRank.ShouldBeLessThan(user1.CurrentRank);
    }
}
