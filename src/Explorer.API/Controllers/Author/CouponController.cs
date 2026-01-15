using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author;

[Authorize(Policy = "authorPolicy")]
[Route("api/author/coupons")]
[ApiController]
public class CouponController : ControllerBase
{
    private readonly ICouponService _couponService;

    public CouponController(ICouponService couponService)
    {
        _couponService = couponService;
    }

    [HttpPost]
    public ActionResult<CouponDto> Create([FromBody] CreateCouponDto dto)
    {
        var authorId = User.PersonId();
        var result = _couponService.Create(authorId, dto);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public ActionResult<CouponDto> Update(long id, [FromBody] CreateCouponDto dto)
    {
        var authorId = User.PersonId();
        var result = _couponService.Update(authorId, id, dto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(long id)
    {
        var authorId = User.PersonId();
        _couponService.Delete(authorId, id);
        return Ok();
    }

    [HttpGet]
    public ActionResult<List<CouponDto>> GetAll()
    {
        var authorId = User.PersonId();
        var result = _couponService.GetByAuthor(authorId);
        return Ok(result);
    }

    [HttpGet("by-author/{authorId}")]
    public ActionResult<List<CouponDto>> GetByAuthorId(long authorId)
    {
        var result = _couponService.GetByAuthor(authorId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public ActionResult<CouponDto> Get(long id)
    {
        var result = _couponService.Get(id);
        return Ok(result);
    }

    [HttpGet("validate/{code}")]
    public ActionResult<CouponDto> ValidateCoupon(string code)
    {
        var result = _couponService.ValidateCoupon(code);
        if (result == null)
            return NotFound("Coupon is invalid or expired");
        return Ok(result);
    }
}
