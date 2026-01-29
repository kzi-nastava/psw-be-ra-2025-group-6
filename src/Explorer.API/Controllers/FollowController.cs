using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Services;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers;

[Authorize]
[Route("api/following")]
[ApiController]
public class FollowController : ControllerBase
{
    private readonly IFollowService _followService;

    public FollowController(IFollowService followService)
    {
        _followService = followService;
    }

    [HttpPost("follow/{followedId:long}")]
    public ActionResult<FollowDto> Follow(long followedId)
    {
        var followerId = User.PersonId();
        try
        {
            var result = _followService.Follow(followerId, followedId);
            return Ok(result);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("unfollow/{followedId:long}")]
    public ActionResult Unfollow(long followedId)
    {
        var followerId = User.PersonId();
        _followService.Unfollow(followerId, followedId);
        return NoContent();
    }

    [HttpGet("followers/{userId:long}")]
    public ActionResult<List<UserDto>> GetFollowers(long userId)
    {
        var followers = _followService.GetFollowers(userId);
        return Ok(followers);
    }

    [HttpGet("following/{userId:long}")]
    public ActionResult<List<UserDto>> GetFollowing(long userId)
    {
        var following = _followService.GetFollowing(userId);
        return Ok(following);
    }

    [HttpGet("followers-count/{userId:long}")]
    public ActionResult<int> GetFollowersCount(long userId)
    {
        var count = _followService.GetFollowersCount(userId);
        return Ok(count);
    }

    [HttpGet("following-count/{userId:long}")]
    public ActionResult<int> GetFollowingCount(long userId)
    {
        var count = _followService.GetFollowingCount(userId);
        return Ok(count);
    }

    [HttpGet("is-following/{followedId:long}")]
    public ActionResult<bool> IsFollowing(long followedId)
    {
        var followerId = User.PersonId();
        var result = _followService.IsFollowing(followerId, followedId);
        return Ok(result);
    }
}
