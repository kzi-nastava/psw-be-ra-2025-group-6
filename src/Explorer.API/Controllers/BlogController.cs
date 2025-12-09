using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public.Administration;
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
                                                        [FromForm] List<IFormFile>? images)
    {
        var userId = User.PersonId();
        var userRole = User.Role();
        if (userRole != UserRole.Author && userRole != UserRole.Tourist)
            return Forbid();

        var blogDto = new BlogCreateDto { Title = title, Description = description };
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
        var userId = User.PersonId();
        var blog = _blogService.GetById(id);

        if (blog == null || blog.UserId != userId)
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
}
