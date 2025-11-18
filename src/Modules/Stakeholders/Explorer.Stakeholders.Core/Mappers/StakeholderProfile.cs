using AutoMapper;
using Explorer.Stakeholders.API.Dtos.ReviewAppDtos;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Mappers;

public class StakeholderProfile : Profile
{
    public StakeholderProfile()
    {
        CreateMap<ReviewAppDto, ReviewApp>().ReverseMap();
    }
}