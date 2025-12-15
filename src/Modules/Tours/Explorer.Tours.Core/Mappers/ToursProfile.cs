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
        CreateMap<TravelType, TravelTypeDto>().ReverseMap();
        CreateMap<TourDuration, TourDurationDto>().ReverseMap();
        CreateMap<TourDto, Tour>()
            .ForMember(dest => dest.PublishedTime, opt => opt.Ignore())
            .ReverseMap(); 
        CreateMap<MonumentDto, Monument>().ReverseMap();
        CreateMap<MeetupDto, Meetup>().ReverseMap();
        CreateMap<KeyPointDto, KeyPoint>().ReverseMap();
    }
}