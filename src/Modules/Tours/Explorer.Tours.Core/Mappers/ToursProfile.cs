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
        CreateMap<Tour, TourDto>().ReverseMap();
        CreateMap<MonumentDto, Monument>().ReverseMap();
        CreateMap<MeetupDto, Meetup>().ReverseMap();
        CreateMap<Quiz, QuizDto>().ReverseMap();
        CreateMap<QuizQuestion, QuizQuestionDto>().ReverseMap();
        CreateMap<QuizAnswerOption, QuizAnswerOptionDto>().ReverseMap();
    }
}