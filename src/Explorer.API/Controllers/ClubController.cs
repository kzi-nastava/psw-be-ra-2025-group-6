using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Explorer.API.Controllers
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
            var result = _clubService.Create(club);
            return Ok(result);
        }

        [HttpPut("{id:long}")]
        public ActionResult<ClubDto> Update([FromBody] ClubDto club)
        {
            var result = _clubService.Update(club);
            return Ok(result);
        }

        [HttpDelete("{id:long}")]
        public ActionResult Delete(long id)
        {
            _clubService.Delete(id);
            return Ok();
        }
    }
}
