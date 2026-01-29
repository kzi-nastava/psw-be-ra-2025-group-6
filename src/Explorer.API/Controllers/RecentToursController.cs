using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.API.Public.Execution;
using Explorer.Tours.Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Explorer.API.Controllers;

[Route("api/recent-tours")]
[ApiController]
public class RecentToursController : ControllerBase
{
    private readonly ITourService _tourService;
    private readonly ITourExecutionService _tourExecutionService;

    public RecentToursController(ITourService tourService, ITourExecutionService tourExecutionService)
    {
        _tourService = tourService;
        _tourExecutionService = tourExecutionService;
    }

    [HttpGet("author/{id:long}")]
    public ActionResult<List<TourDto>> GetAuthorsTours(long id)
    {
        try
        {
            Debug.WriteLine("GetAuthorsTours method called in RecentToursController");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception in GetAuthorsTours method: {ex.Message}");
        }
        return Ok(_tourService.GetPublishedByAuthorId(id));
    }

    [HttpGet("tourist/{id:long}")]
    public ActionResult<List<TourExecutionResultDto>> GetTouristsTours(long id)
    {
        try
        {
            Debug.WriteLine("GetTouristsTours method called in RecentToursController");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception in GetTouristsTours method: {ex.Message}");
        }
        return Ok(_tourExecutionService.GetRecentExecutedTours(id));
    }


}
