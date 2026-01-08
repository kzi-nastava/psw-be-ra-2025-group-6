﻿using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.Quiz;

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
        CreateMap<TourReview, TourReviewDto>().ReverseMap();
        CreateMap<KeyPointDto, KeyPoint>().ReverseMap();
        
        // Public entity request mappings
        CreateMap<PublicEntityRequest, PublicEntityRequestDto>().ReverseMap();
        CreateMap<PublicEntityType, PublicEntityTypeDto>().ReverseMap();
        CreateMap<RequestStatus, RequestStatusDto>().ReverseMap();

        CreateMap<KeyPoint, PublicKeyPointDto>().ReverseMap();
        CreateMap<Facility, PublicFacilityDto>().ReverseMap();
        CreateMap<OrderItem, OrderItemDto>().ReverseMap();
        CreateMap<Quiz, QuizDto>().ReverseMap();
        CreateMap<QuizQuestion, QuizQuestionDto>().ReverseMap();
        CreateMap<QuizAnswerOption, QuizAnswerOptionDto>().ReverseMap();
        CreateMap<Tour, TouristTourDto>()
           .ForMember(dest => dest.FirstKeyPoint, opt => opt.MapFrom(src => src.GetFirstKeyPoint()))
           .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration));
    }
}