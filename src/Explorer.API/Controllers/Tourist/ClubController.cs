using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/community/clubs")]
    public class ClubController : ControllerBase
    {
        private readonly IClubService _clubService;

        public ClubController(IClubService clubService)
        {
            _clubService = clubService;
        }

        [HttpGet]
        public ActionResult<List<ClubDto>> GetAll()
        {
            var result = _clubService.GetAll();
            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public ActionResult<ClubDto> Get(long id)
        {
            var result = _clubService.Get(id);
            return Ok(result);
        }

        [HttpPost]
        public ActionResult<ClubDto> Create([FromBody] ClubDto club)
        {
            var userId = long.Parse(User.FindFirst("id").Value);

            club.OwnerId = userId;
            var result = _clubService.Create(club);
            return Ok(result);
        }

        [HttpPut("{id:long}")]
        public ActionResult<ClubDto> Update([FromBody] ClubDto club)
        {
            var userId = long.Parse(User.FindFirst("id").Value);
           
            if (club.OwnerId != userId)
            {
                return Forbid(); // Vraća 403 Forbidden ako niste vlasnik
            }
            var result = _clubService.Update(club);
            return Ok(result);
        }

        [HttpDelete("{id:long}")]
        public ActionResult Delete(long id)
        {
            var clubToDelete = _clubService.Get(id);
            if (clubToDelete == null) return NotFound();

            var userId = long.Parse(User.FindFirst("id").Value);
            if (clubToDelete.OwnerId != userId)
            {
                return Forbid();
            }
            _clubService.Delete(id);
            return Ok();
        }
    }
}
