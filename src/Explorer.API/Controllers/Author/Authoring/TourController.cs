using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
        var userId = User.PersonId();
        tour.AuthorId = userId;
        return Ok(_tourService.Create(tour));
    }

    [HttpPut("{id:long}")]
    public ActionResult<TourDto> Update(long id, [FromBody] TourDto tour)
    {
        var existingTour = _tourService.Get(id);
        if (existingTour.AuthorId != User.PersonId())
        {
            throw new ForbiddenException("You're not allowed to edit this tour");
        }

        tour.Id = id; // Ensure the DTO id matches the route id
        tour.AuthorId = User.PersonId();
        return Ok(_tourService.Update(tour));
    }


    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        if (_tourService.Get(id).AuthorId != User.PersonId())
        {
            throw new ForbiddenException("You're not allowed to delete this tour");
        }

        _tourService.Delete(id);
        return Ok();
    }

    [HttpPut("{tourId}/equipment/{equipmentId}")]
    public ActionResult AddEquipmentToTour(long tourId, long equipmentId)
    {
        _tourService.AddEquipmentToTour(tourId, equipmentId);
        return Ok();
    }

}
