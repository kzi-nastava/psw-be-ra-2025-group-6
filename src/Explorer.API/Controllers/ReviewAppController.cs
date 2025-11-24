using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos.ReviewAppDtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers
{
    [Route("api/review-app")]
    [ApiController]
    public class ReviewAppController : ControllerBase
    {
        private readonly IReviewAppService _reviewService;

        public ReviewAppController(IReviewAppService reviewService)
        {
            _reviewService = reviewService;
        }

        [Authorize(Policy = "reviewAdminPolicy")]
        [HttpGet("all")]
        public ActionResult<List<ReviewAppDto>> GetAll()
        {
            return Ok(_reviewService.GetAll());
        }
        [Authorize(Policy = "reviewAdminPolicy")]
        [HttpGet("paged")]
        public ActionResult<PagedResult<ReviewAppDto>> GetPaged(
            [FromQuery] int page,
            [FromQuery] int pageSize)
        {
            return Ok(_reviewService.GetPaged(page, pageSize));
        }
        [Authorize(Policy = "reviewAuthorTouristPolicy")]
        [HttpGet("user")]
        public ActionResult<List<ReviewAppDto>> GetByUser()
        {
            var userId = long.Parse(User.Claims.First(c => c.Type == "id").Value);
            return Ok(_reviewService.GetByUser(userId));
        }

        [Authorize(Policy = "reviewAuthorTouristPolicy")]
        [HttpPost]
        public ActionResult<ReviewAppDto> Create([FromBody] CreateReviewAppDto dto)
        {
            var userId = long.Parse(User.Claims.First(c => c.Type == "id").Value);
            return Ok(_reviewService.Create(dto, userId));
        }
        [Authorize(Policy = "reviewAuthorTouristPolicy")]
        [HttpPut("{id:long}")]
        public ActionResult<ReviewAppDto> Update(long id, [FromBody] UpdateReviewAppDto dto)
        {
            var userId = long.Parse(User.Claims.First(c => c.Type == "id").Value);
            return Ok(_reviewService.Update(id, dto, userId));
        }

        [Authorize(Policy = "reviewAuthorTouristPolicy")]
        [HttpDelete("{id:long}")]
        public IActionResult Delete(long id)
        {
            var userId = long.Parse(User.Claims.First(c => c.Type == "id").Value);
            _reviewService.Delete(id, userId);
            return Ok("Review deleted successfully.");
        }


    }

}