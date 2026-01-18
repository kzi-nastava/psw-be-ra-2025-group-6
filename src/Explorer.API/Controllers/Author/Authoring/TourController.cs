using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.API.Contracts;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Core.Domain;
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
        try
        {
            Debug.WriteLine("GetAll method called in TourController");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception in GetAll method: {ex.Message}");
        }
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
            return StatusCode(StatusCodes.Status403Forbidden, ApiErrorFactory.Create(HttpContext, ApiErrorCodes.Forbidden, "Editing this tour is not allowed.", "You're not allowed to edit this tour."));
        }

        tour.Id = id;
        tour.AuthorId = User.PersonId();
        return Ok(_tourService.Update(tour));
    }


    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        if (_tourService.Get(id).AuthorId != User.PersonId())
        {
            return StatusCode(StatusCodes.Status403Forbidden, ApiErrorFactory.Create(HttpContext, ApiErrorCodes.Forbidden, "Deleting this tour is not allowed.", "You're not allowed to delete this tour."));
        }

        _tourService.Delete(id);
        return Ok();
    }

    [HttpPut("{id:long}/archive")]
    public ActionResult<TourDto> Archive(long id)
    {
        var result = _tourService.Archive(id, User.PersonId());
        return Ok(result);
    }

    [HttpPut("{id:long}/activate")]
    public ActionResult<TourDto> Activate(long id)
    {
        var result = _tourService.Activate(id, User.PersonId());
        return Ok(result);
    }



    [HttpPut("{tourId}/add-equipment/{equipmentId}")]
    public ActionResult AddEquipmentToTour(long tourId, long equipmentId)
    {
        _tourService.AddEquipmentToTour(tourId, equipmentId);
        return Ok();
    }

    [HttpPut("{tourId}/remove-equipment/{equipmentId}")]
    public ActionResult RemoveEquipmentFromTour(long tourId, long equipmentId)
    {
        _tourService.RemoveEquipmentFromTour(tourId, equipmentId);
        return Ok();
    }

    [HttpPost("{tourId:long}/key-points")]
    public ActionResult<TourDto> AddKeyPoint(long tourId, [FromBody] KeyPointDto keyPoint)
    {
        var tour = _tourService.Get(tourId);
        if (tour.AuthorId != User.PersonId())
        {
            return StatusCode(StatusCodes.Status403Forbidden, ApiErrorFactory.Create(HttpContext, ApiErrorCodes.Forbidden, "Adding key points is not allowed.", "You're not allowed to add key points to this tour."));
        }

        keyPoint.TourId = tourId;
        var result = _tourService.AddKeyPoint(tourId, keyPoint);
        return Ok(result);
    }
    [HttpPut("{tourId}/distance")]
    public ActionResult<TourDto> UpdateDistance(long tourId, [FromQuery] double distance)
    {
        var tour = _tourService.Get(tourId);
        if (tour.AuthorId != User.PersonId())
        {
            return StatusCode(StatusCodes.Status403Forbidden, ApiErrorFactory.Create(HttpContext, ApiErrorCodes.Forbidden, "Editing this tour is not allowed.", "You're not allowed to edit this tour."));
        }

        var result = _tourService.UpdateTourDistance(tourId, distance);
        return Ok(result);
    }

    [HttpPut("{tourId}/durations")]
    public ActionResult<TourDto> UpdateDurations(long tourId, [FromBody] List<TourDurationDto> durations)
    {
        var tour = _tourService.Get(tourId);
        if (tour.AuthorId != User.PersonId())
            return StatusCode(StatusCodes.Status403Forbidden, ApiErrorFactory.Create(HttpContext, ApiErrorCodes.Forbidden, "Editing this tour is not allowed.", "You're not allowed to edit this tour."));

        var result = _tourService.UpdateDuration(tourId, durations);
        return Ok(result);
    }
    [HttpPut("{id:long}/publish")]
    public ActionResult<TourDto> Publish(long id)
    {
        var result = _tourService.Publish(id, User.PersonId());
        return Ok(result);
    }



}
