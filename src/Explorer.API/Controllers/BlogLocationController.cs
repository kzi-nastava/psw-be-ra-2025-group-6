using Explorer.Blog.API.Public.Administration;
using Explorer.Blog.API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;

namespace Explorer.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/blog-locations")]
    public class BlogLocationController : ControllerBase
    {
        private readonly IBlogLocationService _locationService;

        public BlogLocationController(IBlogLocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpPost]
        public ActionResult<BlogLocationDto> CreateOrGetLocation([FromBody] BlogLocationDto dto)
        {
            try
            {
                var location = _locationService.CreateOrGet(dto);
                return Ok(location);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:long}")]
        public ActionResult<BlogLocationDto> GetById(long id)
        {
            try
            {
                var location = _locationService.GetById(id);
                return Ok(location);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<List<BlogLocationDto>> GetAll()
        {
            try
            {
                var locations = _locationService.GetAll();
                return Ok(locations);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
