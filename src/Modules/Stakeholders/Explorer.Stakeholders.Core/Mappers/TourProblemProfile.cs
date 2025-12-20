using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;
using System;

namespace Explorer.Tours.Core.Mappers
{
    public class TourProblemProfile : Profile
    {
        public TourProblemProfile()
        {
            CreateMap<TourProblem, TourProblemDto>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => (int)src.Category))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => (int)src.Priority))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
                .ForMember(dest => dest.ResolutionFeedback, opt => opt.MapFrom(src => (int)src.ResolutionFeedback))
                .ForMember(dest => dest.ResolutionComment, opt => opt.MapFrom(src => src.ResolutionComment))
                .ForMember(dest => dest.ResolutionAt, opt => opt.MapFrom(src => src.ResolutionAt))
                .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.IsOverdue(DateTime.UtcNow)))
                .ReverseMap()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => (ProblemCategory)src.Category))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => (ProblemPriority)src.Priority))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (ProblemStatus)src.Status))
                .ForMember(dest => dest.ResolutionFeedback, opt => opt.MapFrom(src => (ProblemResolutionFeedback)src.ResolutionFeedback))
                .ForMember(dest => dest.ResolutionComment, opt => opt.MapFrom(src => src.ResolutionComment))
                .ForMember(dest => dest.ResolutionAt, opt => opt.MapFrom(src => src.ResolutionAt));
        }
    }
}
