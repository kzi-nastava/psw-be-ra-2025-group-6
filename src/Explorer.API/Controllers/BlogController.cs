using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public.Administration;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers;

[Authorize]
[Route("api/blogs")]
[ApiController]
public class BlogController : ControllerBase
{
    private readonly IBlogService _blogService;

    public BlogController(IBlogService blogService)
    {
        _blogService = blogService;
    }

    [HttpGet("my-blogs")]
    public ActionResult<List<BlogDto>> GetUserBlogs()
    {
        var userId = User.PersonId();
        var blogs = _blogService.GetByUser(userId);
        return Ok(blogs);
    }

    [HttpPost]
    public async Task<ActionResult<BlogDto>> CreateBlog([FromForm] string title,
                                                        [FromForm] string description,
                                                        [FromForm] List<IFormFile>? images,
                                                        [FromForm] BlogStatusDto status,
                                                        [FromForm] string? city = null,
                                                        [FromForm] string? country = null,
                                                        [FromForm] string? region = null,
                                                        [FromForm] double? latitude = null,
                                                        [FromForm] double? longitude = null)
    {
        var userId = User.PersonId();
        var userRole = User.Role();
        if (userRole != UserRole.Author && userRole != UserRole.Tourist)
            return Forbid();

        var blogDto = new BlogCreateDto { 
            Title = title, 
            Description = description, 
            Status = status,
            City = city,
            Country = country,
            Region = region,
            Latitude = latitude,
            Longitude = longitude
        };
        var createdBlog = _blogService.Create(blogDto, userId);

        if (images == null || !images.Any())
            return Ok(createdBlog);

        var root = Directory.GetCurrentDirectory();
        var folder = Path.Combine(root, "wwwroot/images/blogs");
        Directory.CreateDirectory(folder);

        var imagePaths = new List<string>();

        foreach (var image in images)
        {
            var fileName = $"{Guid.NewGuid()}_{image.FileName}";
            var path = Path.Combine(folder, fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            imagePaths.Add($"/images/blogs/{fileName}");
        }

        _blogService.AddImages(createdBlog.Id, imagePaths);
        createdBlog.Images = imagePaths;
        return Ok(createdBlog);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BlogDto>> UpdateBlog(int id,
                                                        [FromForm] string title,
                                                        [FromForm] string description,
                                                        [FromForm] BlogStatusDto status,
                                                        [FromForm] List<IFormFile>? images = null)
    {
        var userId = User.PersonId();
        var blogDto = new BlogDto
        {
            Id = id,
            Title = title,
            Description = description,
            Status = status,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            Images = new List<string>(),
            LastModifiedAt = DateTime.UtcNow
        };

        var updated = _blogService.Update(blogDto);

        if (images != null && images.Any())
        {
            var root = Directory.GetCurrentDirectory();
            var folder = Path.Combine(root, "wwwroot/images/blogs");
            Directory.CreateDirectory(folder);

            var imagePaths = new List<string>();

            foreach (var image in images)
            {
                var fileName = $"{Guid.NewGuid()}_{image.FileName}";
                var path = Path.Combine(folder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                imagePaths.Add($"/images/blogs/{fileName}");
            }

            _blogService.AddImages(id, imagePaths);
            updated.Images.AddRange(imagePaths);
        }

        return Ok(updated);
    }

    [HttpGet("{id:long}")]
    public ActionResult<BlogDto> GetBlog(long id)
    {
        var blog = _blogService.GetById(id);

        if (blog == null)
        {
            return NotFound();
        }

        return Ok(blog);
    }

    [HttpDelete("{id:long}")]
    public ActionResult DeleteBlog(long id)
    {
        var userId = User.PersonId();

        var blog = _blogService.GetById(id);
        if (blog == null || blog.UserId != userId)
            return NotFound();

        _blogService.Delete(id);

        return NoContent();
    }

    [HttpPost("{id:long}/comments")]
    public IActionResult AddComment(long id, [FromBody] CommentCreateDto dto)
    {
        var userId = User.PersonId();
        var created = _blogService.AddComment(id, userId, dto.Text);

        return Ok(created);
    }

    [HttpPut("{id:long}/comments/{commentId:int}")]
    public IActionResult EditComment(long id, int commentId, [FromBody] CommentCreateDto dto)
    {
        var userId = User.PersonId();
        _blogService.EditComment(id, commentId, userId, dto.Text);

        return Ok();
    }

    [HttpDelete("{id:long}/comments/{commentId:int}")]
    public IActionResult DeleteComment(long id, int commentId)
    {
        var userId = User.PersonId();
        _blogService.DeleteComment(id, commentId, userId);

        return NoContent();
    }

    [HttpGet("{id:long}/comments")]
    public IActionResult GetComment(long id)
    {
        var comments = _blogService.GetComments(id);
        return Ok(comments);
    }

    [HttpPost("{id:long}/vote")]
    public IActionResult VoteOnBlog(long id, [FromBody] VoteRequestDto request)
    {
        var userId = User.PersonId();

        if (request.VoteType == null)
            _blogService.RemoveVote(userId, id);
        else
            _blogService.Vote(userId, id, request.VoteType.Value);

        var votes = _blogService.GetVotes(id);
        var userVote = _blogService.GetUserVote(userId, id);
        return Ok(new { votes.upvotes, votes.downvotes, userVote });
    }

    [HttpDelete("{id:long}/vote")]
    public IActionResult RemoveVote(long id)
    {
        var userId = User.PersonId();
        _blogService.RemoveVote(userId, id);
        var votes = _blogService.GetVotes(id);
        var userVote = _blogService.GetUserVote(userId, id);
        return Ok(new { votes.upvotes, votes.downvotes, userVote });
    }

    [HttpGet("{id:long}/votes")]
    public IActionResult GetVotes(long id)
    {
        var votes = _blogService.GetVotes(id);
        var userId = User.PersonId();
        var userVote = _blogService.GetUserVote(userId, id);
        return Ok(new { votes.upvotes, votes.downvotes, userVote });
    }

    [HttpPatch("{id:long}/archive")]
    public IActionResult ArchiveBlog(long id)
    {
        try
        {
            _blogService.Archive(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("paged")]
    public ActionResult<PagedResult<BlogDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
    {
        return Ok(_blogService.GetPaged(page, pageSize));
    }

    [HttpPatch("{id:long}/description")]
    public ActionResult<BlogDto> UpdateBlogDescription(long id, [FromBody] BlogDescriptionUpdateDto dto)
    {
        try
        {
            var updated = _blogService.UpdateDescription(id, dto.NewDescription);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("{id:long}/quality")]
    public IActionResult RecalculateQualityStatus(long id)
    {
        try
        {
            var updated = _blogService.RecalculateQualityStatus(id);
            return Ok(updated);
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("filter-by-quality")]
    public ActionResult<List<BlogDto>> GetBlogsByQuality([FromQuery] BlogQualityStatusDto status)
    {
        try
        {
            var blogs = _blogService.GetBlogsByQualityStatus(status);
            return Ok(blogs);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
