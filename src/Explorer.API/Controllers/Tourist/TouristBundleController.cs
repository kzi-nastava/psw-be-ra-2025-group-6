using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
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
        var result = _bundleService.GetPublished();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public ActionResult<BundleDto> Get(long id)
    {
        var result = _bundleService.Get(id);
        return Ok(result);
    }
}
