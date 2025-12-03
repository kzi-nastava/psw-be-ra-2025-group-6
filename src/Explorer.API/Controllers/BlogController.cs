using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public.Administration;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
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
                                                        [FromForm] BlogStatusDto status)
    {
        var userId = User.PersonId();
        var userRole = User.Role();
        if (userRole != UserRole.Author && userRole != UserRole.Tourist)
            return Forbid();

        var blogDto = new BlogCreateDto { Title = title, Description = description, Status = status };
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
    public ActionResult<BlogDto> UpdateBlog(long id, [FromBody] BlogDto dto)
    {
        var userId = User.PersonId();
        var updated = _blogService.Update(dto);
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
    public IActionResult DeleteBlog(long id)
    {
        var userId = User.PersonId();

        var blog = _blogService.GetById(id);
        if (blog == null || blog.UserId != userId)
            return NotFound();

        _blogService.Delete(id);

        return NoContent();
    }

    [HttpPatch("{id:long}/archive")]
    public ActionResult<BlogDto> ArchiveBlog(long id)
    {
        try
        {
            _blogService.Archive(id);
            return Ok();
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
}
