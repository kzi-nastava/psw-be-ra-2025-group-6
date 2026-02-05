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
        // Convert ValidUntil to UTC if provided
        var validUntil = dto.ValidUntil.HasValue 
            ? ConvertToUtc(dto.ValidUntil.Value)
            : (DateTime?)null;
        
        var coupon = new Coupon(authorId, dto.DiscountPercent, dto.TourId, validUntil);
        var result = _couponRepository.Create(coupon);
        return _mapper.Map<CouponDto>(result);
    }

    public CouponDto Update(long authorId, long couponId, CreateCouponDto dto)
    {
        var existingCoupon = _couponRepository.Get(couponId);

        if (existingCoupon.AuthorId != authorId)
            throw new ForbiddenException("You can only update your own coupons");

        // Convert ValidUntil to UTC if provided
        var validUntil = dto.ValidUntil.HasValue 
            ? ConvertToUtc(dto.ValidUntil.Value)
            : (DateTime?)null;

        existingCoupon.Update(dto.DiscountPercent, dto.TourId, validUntil);
        var result = _couponRepository.Update(existingCoupon);
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

    /// <summary>
    /// Converts a DateTime to UTC. If the DateTime is already in UTC kind, returns as-is.
    /// If it's unspecified (from JSON), assumes it's UTC and sets the kind.
    /// </summary>
    private static DateTime ConvertToUtc(DateTime dateTime)
    {
        return dateTime.Kind switch
        {
            DateTimeKind.Utc => dateTime,
            DateTimeKind.Local => dateTime.ToUniversalTime(),
            DateTimeKind.Unspecified => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc),
            _ => dateTime
        };
    }
}
