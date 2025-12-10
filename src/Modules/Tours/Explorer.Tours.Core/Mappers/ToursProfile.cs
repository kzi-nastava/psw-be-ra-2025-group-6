using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;
using System.Linq;

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
        CreateMap<Tour, TourDto>().ReverseMap();
        CreateMap<Tour, TourSummaryDto>()
            .ForMember(d => d.FirstKeyPointLatitude,
                opt => opt.MapFrom(s => s.KeyPoints != null
                    ? s.KeyPoints.Select(kp => (double?)kp.Latitude).FirstOrDefault()
                    : null))
            .ForMember(d => d.FirstKeyPointLongitude,
                opt => opt.MapFrom(s => s.KeyPoints != null
                    ? s.KeyPoints.Select(kp => (double?)kp.Longitude).FirstOrDefault()
                    : null));
        CreateMap<MonumentDto, Monument>().ReverseMap();
        CreateMap<MeetupDto, Meetup>().ReverseMap();
        CreateMap<KeyPointDto, KeyPoint>().ReverseMap();
    }
}
