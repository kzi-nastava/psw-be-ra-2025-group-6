using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;

using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos.ReviewAppDtos;

using Explorer.Stakeholders.API.Dtos;

using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Mappers;

public class StakeholderProfile : Profile
{
    public StakeholderProfile()
    {
        CreateMap<UserProfileDto, UserProfile>().ReverseMap();

        CreateMap<ReviewAppDto, ReviewApp>().ReverseMap();
        CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));

        CreateMap<ClubDto, Club>().ReverseMap();
        CreateMap<TouristPositionDto, TouristPosition>().ReverseMap();

    }
}