using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Mappers;
public class ToursProfile : Profile
{
    public ToursProfile()
    {
        CreateMap<EquipmentDto, Equipment>().ReverseMap();
        CreateMap<FacilityDto, Facility>().ReverseMap();
        CreateMap<JournalDto, Journal>().ReverseMap();
        CreateMap<AnnualAward, AnnualAwardDto>().ReverseMap();
        CreateMap<AwardStatus, AwardStatusDto>().ReverseMap();
        CreateMap<TouristEquipmentDto, TouristEquipment>().ReverseMap();
        CreateMap<TourDifficulty, TourDifficultyDto>().ReverseMap();
        CreateMap<TourStatus, TourStatusDto>().ReverseMap();
        CreateMap<Tour, TourDto>().ReverseMap();
        CreateMap<MonumentDto, Monument>().ReverseMap();
        CreateMap<MeetupDto, Meetup>().ReverseMap();
        CreateMap<KeyPointDto, KeyPoint>().ReverseMap();
        CreateMap<ShoppingCart, ShoppingCartDto>().ReverseMap();
        CreateMap<OrderItem, OrderItemDto>().ReverseMap();
        CreateMap<TourPurchaseToken, TourPurchaseTokenDto>().ReverseMap();
    }
}