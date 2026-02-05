using Explorer.Encounters.API.Public;
using Explorer.Encounters.API.Dtos; // <-- This using is required for ChallengeDto
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Encounters.Core.UseCases;

namespace Explorer.API.Controllers.Encounters;

[ApiController]
[Route("api/encounters/challenges")]
public class ChallengesController : ControllerBase
{
    private readonly IChallengePublicService _publicService;
    private readonly IChallengeService _adminService;

    public ChallengesController(IChallengePublicService publicService, Explorer.Encounters.Core.UseCases.IChallengeService adminService)
    {
        _publicService = publicService;
        _adminService = adminService;
    }

    // Public: tourists see only active challenges
    [HttpGet]
    public ActionResult<List<ChallengeDto>> GetActive()
    {
        return Ok(_publicService.GetActive());
    }

    [HttpGet("{id:long}")]
    public ActionResult<ChallengeDto> Get(long id)
    {
        return Ok(_publicService.Get(id));
    }

    // Admin: get all challenges (including Draft/Archived)
    [HttpGet("all")]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult<List<ChallengeDto>> GetAll()
    {
        return Ok(_adminService.GetAll());
    }

    // Admin: create challenge
    [HttpPost]
    [Authorize(Policy = "administratorPolicy")]
    public async Task<ActionResult<ChallengeDto>> Create(
        [FromForm] string title,
        [FromForm] string description,
        [FromForm] string longitude,
        [FromForm] string latitude,
        [FromForm] int xp,
        [FromForm] string type,
        [FromForm] string status,
        [FromForm] int activationRadiusMeters,
        IFormFile? image = null)
    {
        Console.WriteLine($"=== BACKEND CREATE ===");
        Console.WriteLine($"Title: '{title}'");
        Console.WriteLine($"Longitude RAW: '{longitude}'");
        Console.WriteLine($"Latitude RAW: '{latitude}'");
        Console.WriteLine($"Image: {image?.FileName ?? "NULL"}");

        try
        {
            if (!double.TryParse(longitude, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var lng))
            {
                return BadRequest(new { message = $"Invalid Longitude format: '{longitude}'" });
            }

            if (!double.TryParse(latitude, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var lat))
            {
                return BadRequest(new { message = $"Invalid Latitude format: '{latitude}'" });
            }

            Console.WriteLine($"Parsed: Longitude={lng}, Latitude={lat}");

            var dto = new ChallengeDto
            {
                Title = title,
                Description = description,
                Longitude = lng,
                Latitude = lat,
                XP = xp,
                Type = type,
                Status = status,
                ActivationRadiusMeters = activationRadiusMeters
            };

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

                dto.ImagePath = $"/uploads/challenges/{fileName}";
                Console.WriteLine($"IMAGE UPLOADED: {dto.ImagePath}");
            }

            var result = _adminService.Create(dto);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"[ERROR] {ex.Message}");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] {ex.Message}");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("create-by-tourist")]
    [Authorize(Policy = "touristPolicy")]
    public async Task<ActionResult<ChallengeDto>> CreateByTourist(
        [FromForm] string title,
        [FromForm] string description,
        [FromForm] string longitude,
        [FromForm] string latitude,
        [FromForm] int xp,
        [FromForm] string type,
        [FromForm] int activationRadiusMeters,
        IFormFile? image)
    {
        var touristId = User.PersonId();
        if (touristId == 0)
        {
            Console.WriteLine("ERROR: PersonId from token is null or 0!");
            return BadRequest(new { message = "Invalid token, cannot determine user ID." });
        }

        try
        {
            if (!double.TryParse(longitude, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var lng))
            {
                return BadRequest(new { message = $"Invalid Longitude format: '{longitude}'" });
            }

            if (!double.TryParse(latitude, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var lat))
            {
                return BadRequest(new { message = $"Invalid Latitude format: '{latitude}'" });
            }

            var dto = new ChallengeDto
            {
                Title = title,
                Description = description,
                Longitude = lng,
                Latitude = lat,
                XP = xp,
                Type = type,
                ActivationRadiusMeters = activationRadiusMeters
            };

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

                dto.ImagePath = $"/uploads/challenges/{fileName}";
            }

            var result = _adminService.CreateByTourist(dto, touristId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($" Error: {ex.Message}"); 
            return BadRequest(new { message = ex.Message });
        }
    }

    // Admin: get challenges pending approval
    [HttpGet("pending-approval")]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult<List<ChallengeDto>> GetPendingApproval()
    {
        var pending = _adminService.GetPendingApproval(); // Only tourist-created Draft challenges
        Console.WriteLine($"[PENDING APPROVAL] Returning {pending.Count} tourist-created Draft challenges");
        return Ok(pending);
    }

    // Admin: approve tourist-created challenge
    [HttpPut("{id:long}/approve")]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult<ChallengeDto> Approve(long id)
    {
        try
        {
            return Ok(_adminService.ApproveChallenge(id));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Admin: reject tourist-created challenge
    [HttpPut("{id:long}/reject")]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult<ChallengeDto> Reject(long id)
    {
        try
        {
            return Ok(_adminService.RejectChallenge(id));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "administratorPolicy")]
    public async Task<ActionResult<ChallengeDto>> Update(
        long id,
        [FromForm] string title,
        [FromForm] string description,
        [FromForm] string longitude,
        [FromForm] string latitude,
        [FromForm] int xp,
        [FromForm] string type,
        [FromForm] string status,
        [FromForm] int activationRadiusMeters,
        IFormFile? image = null)
    {
        Console.WriteLine($"=== BACKEND UPDATE {id} ===");
        Console.WriteLine($"Longitude RAW: '{longitude}'");
        Console.WriteLine($"Latitude RAW: '{latitude}'");

        try
        {
            if (!double.TryParse(longitude, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var lng))
            {
                return BadRequest(new { message = $"Invalid Longitude format: '{longitude}'" });
            }

            if (!double.TryParse(latitude, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var lat))
            {
                return BadRequest(new { message = $"Invalid Latitude format: '{latitude}'" });
            }

            var updateDto = new ChallengeDto
            {
                Id = id,
                Title = title,
                Description = description,
                Longitude = lng,
                Latitude = lat,
                XP = xp,
                Type = type,
                Status = status,
                ActivationRadiusMeters = activationRadiusMeters
            };

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
                var existing = _adminService.Get(id);
                updateDto.ImagePath = existing.ImagePath;
            }

            var result = _adminService.Update(id, updateDto);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"[ERROR] {ex.Message}");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] {ex.Message}");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult Delete(long id)
    {
        _adminService.Delete(id);
        return Ok();
    }
}
