using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author.Authoring;

[Authorize(Policy = "authorPolicy")]
[Route("api/authoring/tours")]
[ApiController]
public class TourController : ControllerBase
{
    private readonly ITourService _tourService;

    public TourController(ITourService tourService)
    {
        _tourService = tourService;
    }

    [HttpGet("paged")]
    public ActionResult<PagedResult<TourDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
    {
        return Ok(_tourService.GetPaged(page, pageSize));
    }
    [HttpGet]
    public ActionResult<List<TourDto>> GetAll()
    {
        return Ok(_tourService.GetAll());
    }

    [HttpGet("{id:long}")]
    public ActionResult<TourDto> Get(long id)
    {
        return Ok(_tourService.Get(id));
    }

    [HttpPost]
    public ActionResult<TourDto> Create([FromBody] TourDto tour)
    {
        return Ok(_tourService.Create(tour));
    }

    [HttpPut("{id:long}")]
    public ActionResult<TourDto> Update([FromBody] TourDto tour)
    {
        return Ok(_tourService.Update(tour));
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        _tourService.Delete(id);
        return Ok();
    }
}
