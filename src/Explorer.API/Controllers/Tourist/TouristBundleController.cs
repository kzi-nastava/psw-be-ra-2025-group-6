using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/bundles")]
[ApiController]
public class TouristBundleController : ControllerBase
{
    private readonly IBundleService _bundleService;

    public TouristBundleController(IBundleService bundleService)
    {
        _bundleService = bundleService;
    }

    [HttpGet]
    public ActionResult<List<BundleDto>> GetPublished()
    {
        var touristId = User.PersonId();
        var result = _bundleService.GetAvailableForTourist(touristId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public ActionResult<BundleDto> Get(long id)
    {
        var result = _bundleService.Get(id);
        return Ok(result);
    }
}
