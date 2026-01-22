using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/tour-reviews")]
    [ApiController]
    public class TourReviewController : ControllerBase
    {
        private ITourReviewService _tourReviewService;
        public TourReviewController(ITourReviewService service)
        {
            _tourReviewService = service;
        }

        [HttpPost]
        public ActionResult<TourReviewDto> Create([FromBody] TourReviewDto reviewDto)
        {
            reviewDto.UserId = User.PersonId();
            var result = _tourReviewService.Create(reviewDto);
            return Ok(result);
        }

        [HttpGet]
        public ActionResult<List<TourReviewDto>> GetMyReviews()
        {
            var userid= User.PersonId();
            var result = _tourReviewService.GetByUser(userid);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public ActionResult<TourReviewDto> Update(long id, [FromBody] TourReviewDto reviewDto)
        {
            reviewDto.Id = id;

            if (IsMyReview(id) == false) return Forbid("You can not update someone elses review");

            var result = _tourReviewService.Update(reviewDto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(long id)
        {
            if (IsMyReview(id) == false) return Forbid("You can not delete someone elses review");

            _tourReviewService.Delete(id);
            return NoContent();
        }

        private bool IsMyReview(long reviewId)
        {
            TourReviewDto? review = _tourReviewService.Get(reviewId);
            if (review == null) return false;

            return review.UserId == User.PersonId();
        }

        [HttpGet("tour/{tourId}")]
        [AllowAnonymous]
        public ActionResult<List<TourReviewDto>> GetByTour(long tourId)
        {
            var result = _tourReviewService.GetByTour(tourId);
            return Ok(result);
        }
    }
}
