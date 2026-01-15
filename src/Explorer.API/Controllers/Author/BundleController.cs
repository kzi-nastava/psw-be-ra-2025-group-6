using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author;

[Authorize(Policy = "authorPolicy")]
[Route("api/author/bundles")]
[ApiController]
public class BundleController : ControllerBase
{
    private readonly IBundleService _bundleService;

    public BundleController(IBundleService bundleService)
    {
        _bundleService = bundleService;
    }

    [HttpPost]
    public ActionResult<BundleDto> Create([FromBody] CreateBundleDto dto)
    {
        var authorId = User.PersonId();
        var result = _bundleService.Create(authorId, dto);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public ActionResult<BundleDto> Update(long id, [FromBody] UpdateBundleDto dto)
    {
        var authorId = User.PersonId();
        var result = _bundleService.Update(authorId, id, dto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(long id)
    {
        var authorId = User.PersonId();
        _bundleService.Delete(authorId, id);
        return Ok();
    }

    [HttpGet]
    public ActionResult<List<BundleDto>> GetAll()
    {
        var authorId = User.PersonId();
        var result = _bundleService.GetByAuthor(authorId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public ActionResult<BundleDto> Get(long id)
    {
        var result = _bundleService.Get(id);
        return Ok(result);
    }

    [HttpPost("{id}/publish")]
    public ActionResult<BundleDto> Publish(long id)
    {
        var authorId = User.PersonId();
        var result = _bundleService.Publish(authorId, id);
        return Ok(result);
    }

    [HttpPost("{id}/archive")]
    public ActionResult<BundleDto> Archive(long id)
    {
        var authorId = User.PersonId();
        var result = _bundleService.Archive(authorId, id);
        return Ok(result);
    }

    [HttpPost("total-price")]
    public ActionResult<double> GetTotalPrice([FromBody] List<long> tourIds)
    {
        var totalPrice = _bundleService.GetTotalToursPrice(tourIds);
        return Ok(totalPrice);
    }
}
