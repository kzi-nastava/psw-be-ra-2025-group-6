using System.Collections.Generic;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public.Administration;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/community/clubs/{clubId:long}/posts")]
    public class ClubPostController : ControllerBase
    {
        private readonly IClubPostService _clubPostService;
        private readonly ITourService _tourService;
        private readonly IBlogService _blogService;

        public ClubPostController(IClubPostService clubPostService, ITourService tourService, IBlogService blogService)
        {
            _clubPostService = clubPostService;
            _tourService = tourService;
            _blogService = blogService;
        }

        [HttpGet]
        public ActionResult<List<ClubPostDto>> GetForClub(long clubId)
        {
            var result = _clubPostService.GetForClub(clubId);
            return Ok(result);
        }

        [HttpPost]
        public ActionResult<ClubPostDto> Create(long clubId, [FromBody] ClubPostDto post)
        {
            if (post == null)
            {
                return BadRequest("Invalid request body.");
            }

            var idClaim = User.FindFirst("id");
            if (idClaim == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            var userId = long.Parse(idClaim.Value);
            post.ClubId = clubId;
            var result = _clubPostService.Create(post, userId);
            return Ok(result);
        }

        [HttpPut("{postId:long}")]
        public ActionResult<ClubPostDto> Update(long clubId, long postId, [FromBody] ClubPostDto post)
        {
            var userId = long.Parse(User.FindFirst("id").Value);
            post.Id = postId;
            post.ClubId = clubId;
            var result = _clubPostService.Update(post, userId);
            return Ok(result);
        }

        [HttpDelete("{postId:long}")]
        public ActionResult Delete(long clubId, long postId)
        {
            var userId = long.Parse(User.FindFirst("id").Value);
            _clubPostService.Delete(postId, userId);
            return Ok();
        }

        [HttpGet("search-resources")]
        public ActionResult<List<ResourceSearchDto>> SearchResources([FromQuery] string query, [FromQuery] int type)
        {
            if (string.IsNullOrWhiteSpace(query)) return Ok(new List<ResourceSearchDto>());

            if (type == 0) // TURE
            {
                var tours = _tourService.GetPublished()
                    .Where(t => t.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .Take(10)
                    .Select(t => new ResourceSearchDto
                    {
                        Id = t.Id,
                        Name = t.Name,
                        AuthorName = $"Author ID: {t.AuthorId}",
                        Image = t.KeyPoints?.FirstOrDefault()?.ImagePath
                    }).ToList();

                return Ok(tours);
            }
            else // BLOGOVI
            {
                var blogs = _blogService.GetPaged(1, 100).Results
                    .Where(b => b.Status == BlogStatusDto.POSTED && 
                                b.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .Take(10)
                    .Select(b => new ResourceSearchDto
                    {
                        Id = (long)b.Id,
                        Name = b.Title,
                        AuthorName = $"by {b.Username}",
                        Image = b.Images?.FirstOrDefault()
                    }).ToList();

                return Ok(blogs);
            }
        }
    }
}
