using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author.Authoring;

[Authorize(Policy = "authorPolicy")]
[Route("api/authoring/public-key-points")]
[ApiController]
public class PublicKeyPointController : ControllerBase
{
    private readonly IKeyPointService _keyPointService;

    public PublicKeyPointController(IKeyPointService keyPointService)
    {
        _keyPointService = keyPointService;
    }

    [HttpGet]
    public ActionResult<List<KeyPointDto>> GetPublicKeyPoints()
    {
        var result = _keyPointService.GetPublicKeyPoints();
        return Ok(result);
    }
}
