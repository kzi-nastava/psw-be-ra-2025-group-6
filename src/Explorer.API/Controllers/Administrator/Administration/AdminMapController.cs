using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration
{
    [Authorize(Policy = "administratorPolicy")]
    [Route("api/administration/map")]
    [ApiController]
    public class AdminMapController : ControllerBase
    {
        private readonly IAdminMapService _service;

        public AdminMapController(IAdminMapService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<List<AdminMapDto>> GetAllMapItems()
        {
            return Ok(_service.GetAllMapItems());
        }

        [HttpPut("{type}/{id:long}")]
        public ActionResult<AdminMapDto> UpdateLocation(string type, long id, [FromBody] AdminUpdateLocationDto dto)
        {
            return Ok(_service.UpdateLocation(type, id, dto));
        }
    }
}
