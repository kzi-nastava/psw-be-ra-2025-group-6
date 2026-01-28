using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tour-planner")]
    [ApiController]
    public class TourCheckpointPlanController : ControllerBase
    {
        private readonly ITourCheckpointPlanService _service;

        public TourCheckpointPlanController(ITourCheckpointPlanService service)
        {
            _service = service;
        }

        private long CurrentUserId() => User.PersonId();

        [HttpGet("{plannerId:long}/checkpoint-plans")]
        public ActionResult<List<TourCheckpointPlanDto>> GetByPlannerItem(long plannerId)
        {
            var result = _service.GetByPlannerItemId(plannerId, CurrentUserId());
            return Ok(result);
        }

        [HttpPost("{plannerId:long}/checkpoint-plans")]
        public ActionResult<TourCheckpointPlanDto> Create(long plannerId, [FromBody] TourCheckpointPlanCreateDto dto)
        {
            if (plannerId != dto.PlannerItemId)
            {
                return BadRequest("Planner item ID in the URL and body do not match.");
            }
            var result = _service.Create(CurrentUserId(), dto);
            return Ok(result);
        }

        [HttpPut("checkpoint-plans/{id:long}")]
        public ActionResult<TourCheckpointPlanDto> Update(long id, [FromBody] TourCheckpointPlanUpdateDto dto)
        {
            var result = _service.Update(id, CurrentUserId(), dto);
            return Ok(result);
        }

        [HttpDelete("checkpoint-plans/{id:long}")]
        public ActionResult Delete(long id)
        {
            _service.Delete(id, CurrentUserId());
            return NoContent();
        }
    }
}
