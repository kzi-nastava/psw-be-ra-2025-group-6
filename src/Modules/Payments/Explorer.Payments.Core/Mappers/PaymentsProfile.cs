using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.Core.Domain;

namespace Explorer.Payments.Core.Mappers;

public class PaymentsProfile : Profile
{
    public PaymentsProfile()
    {
        CreateMap<ShoppingCart, ShoppingCartDto>().ReverseMap();
        CreateMap<OrderItem, OrderItemDto>().ReverseMap();
        CreateMap<TourPurchaseToken, TourPurchaseTokenDto>().ReverseMap();
        
        CreateMap<Bundle, BundleDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<BundleStatus>(src.Status)));
        
        CreateMap<Coupon, CouponDto>().ReverseMap();
        CreateMap<Sale, SaleDto>().ReverseMap();
        CreateMap<PaymentRecord, PaymentRecordDto>().ReverseMap();
    }
}