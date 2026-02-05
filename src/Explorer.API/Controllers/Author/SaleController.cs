using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author;

[Authorize(Policy = "authorPolicy")]
[Route("api/author/sales")]
[ApiController]
public class SaleController : ControllerBase
{
    private readonly ISaleService _saleService;

    public SaleController(ISaleService saleService)
    {
        _saleService = saleService;
    }

    [HttpPost]
    public ActionResult<SaleDto> Create([FromBody] CreateSaleDto dto)
    {
        var authorId = User.PersonId();
        var result = _saleService.Create(authorId, dto);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public ActionResult<SaleDto> Update(long id, [FromBody] CreateSaleDto dto)
    {
        var authorId = User.PersonId();
        var result = _saleService.Update(authorId, id, dto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(long id)
    {
        var authorId = User.PersonId();
        _saleService.Delete(authorId, id);
        return Ok();
    }

    [HttpGet("by-author/{authorId}")]
    public ActionResult<List<SaleDto>> GetByAuthorId(long authorId)
    {
        var result = _saleService.GetByAuthor(authorId);
        return Ok(result);
    }

    [HttpGet("active")]
    public ActionResult<List<SaleDto>> GetActiveSales()
    {
        var result = _saleService.GetActiveSales();
        return Ok(result);
    }

    [HttpGet]
    public ActionResult<List<SaleDto>> GetAll()
    {
        var authorId = User.PersonId();
        var result = _saleService.GetByAuthor(authorId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public ActionResult<SaleDto> Get(long id)
    {
        var result = _saleService.Get(id);
        return Ok(result);
    }
}
