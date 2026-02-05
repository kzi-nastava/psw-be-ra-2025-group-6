using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Explorer.API.Controllers.Author.Authoring;

[Authorize]
[Route("api/public/tours")]
[ApiController]
public class PublicTourController : ControllerBase
{
    private readonly ITourService _tourService;

    public PublicTourController(ITourService tourService)
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



}
