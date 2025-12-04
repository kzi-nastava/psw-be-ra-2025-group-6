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
        CreateMap<Tour, TourDto>().ReverseMap();
        CreateMap<Tour, TourSummaryDto>()
            .ForMember(d => d.FirstKeyPointLatitude,
                opt => opt.MapFrom(s => s.KeyPoints.Select(kp => (double?)kp.Latitude).FirstOrDefault()))
            .ForMember(d => d.FirstKeyPointLongitude,
                opt => opt.MapFrom(s => s.KeyPoints.Select(kp => (double?)kp.Longitude).FirstOrDefault()));
        CreateMap<MonumentDto, Monument>().ReverseMap();
        CreateMap<MeetupDto, Meetup>().ReverseMap();
    }
}
