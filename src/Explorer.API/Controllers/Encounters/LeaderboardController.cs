using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Encounters;

[Authorize(Policy = "touristPolicy")]
[ApiController]
[Route("api/leaderboard")]
public class LeaderboardController : ControllerBase
{
    private readonly ILeaderboardService _leaderboardService;

    public LeaderboardController(ILeaderboardService leaderboardService)
    {
        _leaderboardService = leaderboardService;
    }

    private long GetTouristId() => long.Parse(User.FindFirst("id")!.Value);

    /// <summary>
    /// Get tourist leaderboard with optional filtering (day, week, month, all)
    /// </summary>
    [HttpGet("tourists")]
    public async Task<ActionResult<PagedResult<LeaderboardEntryDto>>> GetTouristLeaderboard(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 50,
        [FromQuery] string? filter = null)
    {
        try
        {
            var result = await _leaderboardService.GetTouristLeaderboardAsync(page, pageSize, filter);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get club leaderboard
    /// </summary>
    [HttpGet("clubs")]
    public async Task<ActionResult<PagedResult<ClubLeaderboardDto>>> GetClubLeaderboard(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var result = await _leaderboardService.GetClubLeaderboardAsync(page, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get specific user's leaderboard stats
    /// </summary>
    [HttpGet("tourists/{userId}")]
    public async Task<ActionResult<LeaderboardEntryDto>> GetUserStats(long userId)
    {
        try
        {
            var result = await _leaderboardService.GetUserLeaderboardStatsAsync(userId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get current user's leaderboard stats
    /// </summary>
    [HttpGet("tourists/me")]
    public async Task<ActionResult<LeaderboardEntryDto>> GetMyStats()
    {
        try
        {
            var touristId = GetTouristId();
            var result = await _leaderboardService.GetUserLeaderboardStatsAsync(touristId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get specific club's leaderboard stats
    /// </summary>
    [HttpGet("clubs/{clubId}")]
    public async Task<ActionResult<ClubLeaderboardDto>> GetClubStats(long clubId)
    {
        try
        {
            var result = await _leaderboardService.GetClubLeaderboardStatsAsync(clubId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Update user stats (called internally when completing challenges/tours)
    /// </summary>
    [HttpPost("update-stats")]
    public async Task<ActionResult> UpdateUserStats([FromBody] UpdateLeaderboardStatsDto dto)
    {
        try
        {
            await _leaderboardService.UpdateUserStatsAsync(
                dto.UserId, 
                dto.XpGained, 
                dto.ChallengesCompleted, 
                dto.ToursCompleted, 
                dto.CoinsEarned);
            
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Initialize leaderboard entries for all existing users (one-time setup)
    /// </summary>
    [AllowAnonymous]
    [HttpPost("initialize")]
    public async Task<ActionResult> InitializeLeaderboard()
    {
        try
        {
            await _leaderboardService.InitializeLeaderboardForAllUsersAsync();
            return Ok("Leaderboard initialized for all users");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Recalculate all ranks (admin operation)
    /// </summary>
    [Authorize(Policy = "administratorPolicy")]
    [HttpPost("recalculate")]
    public async Task<ActionResult> RecalculateRanks()
    {
        try
        {
            await _leaderboardService.RecalculateRanksAsync();
            return Ok("Ranks recalculated successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// User joins a club - updates leaderboard
    /// </summary>
    [HttpPost("user-joined-club")]
    public async Task<ActionResult> UserJoinedClub([FromQuery] long userId, [FromQuery] long clubId)
    {
        try
        {
            await _leaderboardService.UserJoinedClubAsync(userId, clubId);
            return Ok($"User {userId} joined club {clubId} - leaderboard updated");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// User leaves their club - updates leaderboard
    /// </summary>
    [HttpPost("user-left-club")]
    public async Task<ActionResult> UserLeftClub([FromQuery] long userId)
    {
        try
        {
            await _leaderboardService.UserLeftClubAsync(userId);
            return Ok($"User {userId} left club - leaderboard updated");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Initialize club leaderboard (called when new club is created)
    /// </summary>
    [Authorize(Policy = "administratorPolicy")]
    [HttpPost("initialize-club")]
    public async Task<ActionResult> InitializeClubLeaderboard([FromQuery] long clubId, [FromQuery] string clubName)
    {
        try
        {
            await _leaderboardService.InitializeClubLeaderboardAsync(clubId, clubName);
            return Ok($"Club leaderboard initialized for club {clubId}");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Initialize all club leaderboards (one-time setup)
    /// </summary>
    [Authorize(Policy = "administratorPolicy")]
    [HttpPost("initialize-all-clubs")]
    public async Task<ActionResult> InitializeAllClubs()
    {
        try
        {
            await _leaderboardService.InitializeAllClubLeaderboardsAsync();
            return Ok("All club leaderboards initialized");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get all members of a club with their stats
    /// </summary>
    [HttpGet("clubs/{clubId}/members")]
    public async Task<ActionResult<List<LeaderboardEntryDto>>> GetClubMembers(long clubId)
    {
        try
        {
            var members = await _leaderboardService.GetClubMembersAsync(clubId);
            return Ok(members);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
