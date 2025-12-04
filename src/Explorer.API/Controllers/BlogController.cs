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
    private readonly IBlogVoteService _blogVoteService;

    public BlogController(IBlogService blogService, IBlogVoteService blogVoteService)
    {
        _blogService = blogService;
        _blogVoteService = blogVoteService;
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

    [HttpPost("{id:long}/vote")]
    public IActionResult VoteOnBlog(long id, [FromBody] VoteTypeDto voteType)
    {
        var userId = User.PersonId();
        _blogVoteService.Vote(userId, id, voteType);
        var votes = _blogVoteService.GetVotes(id);
        return Ok(votes);
    }

    [HttpGet("{id:long}/votes")]
    public IActionResult GetVotes(long id)
    {
        var votes = _blogVoteService.GetVotes(id);
        var userId = User.PersonId();
        var userVote = userId != 0 ? _blogVoteService.GetUserVote(userId, id)?.Type : null;
        return Ok(new { votes.upvotes, votes.downvotes, userVote });
    }
}
