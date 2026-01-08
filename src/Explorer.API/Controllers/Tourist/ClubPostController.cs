using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/community/clubs/{clubId:long}/posts")]
    public class ClubPostController : ControllerBase
    {
        private readonly IClubPostService _clubPostService;

        public ClubPostController(IClubPostService clubPostService)
        {
            _clubPostService = clubPostService;
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
    }
}
