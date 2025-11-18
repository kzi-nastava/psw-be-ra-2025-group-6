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
        [HttpGet("user/{userId:long}")]
        public ActionResult<List<ReviewAppDto>> GetByUser(long userId)
        {
            return Ok(_reviewService.GetByUser(userId));
        }
        [Authorize(Policy = "reviewAuthorTouristPolicy")]
        [HttpPost]
        public ActionResult<ReviewAppDto> Create([FromBody] CreateReviewAppDto dto)
        {
            return Ok(_reviewService.Create(dto));
        }
        [Authorize(Policy = "reviewAuthorTouristPolicy")]
        [HttpPut("{id:long}")]
        public ActionResult<ReviewAppDto> Update(long id, [FromBody] UpdateReviewAppDto dto)
        {
            return Ok(_reviewService.Update(id, dto));
        }
    }

}