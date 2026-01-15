using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases;

public class CouponService : ICouponService
{
    private readonly ICouponRepository _couponRepository;
    private readonly IMapper _mapper;

    public CouponService(ICouponRepository couponRepository, IMapper mapper)
    {
        _couponRepository = couponRepository;
        _mapper = mapper;
    }

    public CouponDto Create(long authorId, CreateCouponDto dto)
    {
        var coupon = new Coupon(authorId, dto.DiscountPercent, dto.TourId, dto.ValidUntil);
        var result = _couponRepository.Create(coupon);
        return _mapper.Map<CouponDto>(result);
    }

    public CouponDto Update(long authorId, long couponId, CreateCouponDto dto)
    {
        var existingCoupon = _couponRepository.Get(couponId);

        if (existingCoupon.AuthorId != authorId)
            throw new ForbiddenException("You can only update your own coupons");

        var updatedCoupon = new Coupon(authorId, dto.DiscountPercent, dto.TourId, dto.ValidUntil);
        var result = _couponRepository.Update(updatedCoupon);
        return _mapper.Map<CouponDto>(result);
    }

    public void Delete(long authorId, long couponId)
    {
        var coupon = _couponRepository.Get(couponId);

        if (coupon.AuthorId != authorId)
            throw new ForbiddenException("You can only delete your own coupons");

        _couponRepository.Delete(couponId);
    }

    public CouponDto Get(long id)
    {
        var coupon = _couponRepository.Get(id);
        return _mapper.Map<CouponDto>(coupon);
    }

    public List<CouponDto> GetByAuthor(long authorId)
    {
        var coupons = _couponRepository.GetByAuthor(authorId);
        return _mapper.Map<List<CouponDto>>(coupons);
    }

    public CouponDto? ValidateCoupon(string code)
    {
        var coupon = _couponRepository.GetByCode(code);
        
        if (coupon == null)
            return null;

        if (!coupon.IsValid())
            return null;

        return _mapper.Map<CouponDto>(coupon);
    }
}
