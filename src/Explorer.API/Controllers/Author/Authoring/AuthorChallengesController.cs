using Explorer.Encounters.API.Public;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.Core.UseCases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.BuildingBlocks.Core.Exceptions;

namespace Explorer.API.Controllers.Author.Authoring;

[Authorize(Policy = "authorPolicy")]
[Route("api/author/challenges")]
[ApiController]
public class AuthorChallengesController : ControllerBase
{
    private readonly IChallengeService _challengeService;

    public AuthorChallengesController(IChallengeService challengeService)
    {
        _challengeService = challengeService;
    }

    /// <summary>
    /// Get all challenges created by the logged-in author for their key points
    /// </summary>
    [HttpGet("my-challenges")]
    public ActionResult<List<ChallengeDto>> GetMyChallenges()
    {
        var authorId = User.PersonId();
        var allChallenges = _challengeService.GetAll();
        
        // Filter challenges where CreatorId matches the author
        var myChallenges = allChallenges
            .Where(c => c.CreatorId == authorId && c.KeyPointId.HasValue)
            .ToList();

        return Ok(myChallenges);
    }

    /// <summary>
    /// Get challenges for a specific key point (only if author owns it)
    /// </summary>
    [HttpGet("keypoint/{keyPointId:long}")]
    public ActionResult<List<ChallengeDto>> GetByKeyPointId(long keyPointId)
    {
        var authorId = User.PersonId();
        var challenges = _challengeService.GetByKeyPointId(keyPointId);
        
        // Verify that author owns these challenges
        if (challenges.Any() && challenges.First().CreatorId != authorId)
        {
            return Forbid();
        }

        return Ok(challenges);
    }

    /// <summary>
    /// Update a challenge (only if author created it)
    /// </summary>
    [HttpPut("{id:long}")]
    public async Task<ActionResult<ChallengeDto>> Update(
        long id,
        [FromForm] string title,
        [FromForm] string description,
        [FromForm] int xp,
        [FromForm] string type,
        [FromForm] bool isRequiredForSecret,
        [FromForm] int activationRadiusMeters,
        [FromForm] int? requiredPeople,
        [FromForm] double? socialRadiusMeters,
        IFormFile? image = null)
    {
        try
        {
            var authorId = User.PersonId();
            
            // Check if challenge exists and belongs to author
            var existing = _challengeService.Get(id);
            if (existing.CreatorId != authorId)
            {
                return Forbid();
            }

            // Verify it's a KeyPoint challenge
            if (!existing.KeyPointId.HasValue)
            {
                return BadRequest(new { message = "Can only update KeyPoint challenges." });
            }

            var updateDto = new ChallengeDto
            {
                Id = id,
                Title = title,
                Description = description,
                Longitude = existing.Longitude, // Keep existing location
                Latitude = existing.Latitude,
                XP = xp,
                Type = type,
                IsRequiredForSecret = isRequiredForSecret,
                ActivationRadiusMeters = activationRadiusMeters,
                RequiredPeople = requiredPeople,
                SocialRadiusMeters = socialRadiusMeters
            };

            // Handle image upload
            if (image != null && image.Length > 0)
            {
                var root = Directory.GetCurrentDirectory();
                var folder = Path.Combine(root, "wwwroot/uploads/challenges");
                Directory.CreateDirectory(folder);

                var fileName = $"{Guid.NewGuid()}_{image.FileName}";
                var path = Path.Combine(folder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                updateDto.ImagePath = $"/uploads/challenges/{fileName}";
            }
            else
            {
                updateDto.ImagePath = existing.ImagePath;
            }

            var result = _challengeService.Update(id, updateDto);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] {ex.Message}");
            return StatusCode(500, new { message = "An error occurred while updating the challenge." });
        }
    }

    /// <summary>
    /// Delete a challenge (only if author created it)
    /// </summary>
    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        try
        {
            var authorId = User.PersonId();
            
            // Check if challenge exists and belongs to author
            var existing = _challengeService.Get(id);
            if (existing.CreatorId != authorId)
            {
                return Forbid();
            }

            // Verify it's a KeyPoint challenge
            if (!existing.KeyPointId.HasValue)
            {
                return BadRequest(new { message = "Can only delete KeyPoint challenges." });
            }

            _challengeService.Delete(id);
            return Ok(new { message = "Challenge deleted successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] {ex.Message}");
            return StatusCode(500, new { message = "An error occurred while deleting the challenge." });
        }
    }

    /// <summary>
    /// Get a single challenge by ID (only if author owns it)
    /// </summary>
    [HttpGet("{id:long}")]
    public ActionResult<ChallengeDto> Get(long id)
    {
        try
        {
            var authorId = User.PersonId();
            var challenge = _challengeService.Get(id);
            
            if (challenge.CreatorId != authorId)
            {
                return Forbid();
            }

            return Ok(challenge);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
