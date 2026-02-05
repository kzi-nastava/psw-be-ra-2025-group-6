using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public;

public interface ICouponService
{
    CouponDto Create(long authorId, CreateCouponDto dto);
    CouponDto Update(long authorId, long couponId, CreateCouponDto dto);
    void Delete(long authorId, long couponId);
    CouponDto Get(long id);
    List<CouponDto> GetByAuthor(long authorId);
    CouponDto? ValidateCoupon(string code);
}
