using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Tours.Core.Mappers
{
    public class TourProblemProfile : Profile
    {
        public TourProblemProfile()
        {
            CreateMap<TourProblem, TourProblemDto>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => (int)src.Category))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => (int)src.Priority))
                .ReverseMap()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => (ProblemCategory)src.Category))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => (ProblemPriority)src.Priority));
        }
    }
}