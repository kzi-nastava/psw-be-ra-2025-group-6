using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Dtos.ReviewAppDtos;
using Explorer.Stakeholders.Core.Domain;


namespace Explorer.Stakeholders.Core.Mappers;

public class StakeholderProfile : Profile
{
    public StakeholderProfile()
    {
        CreateMap<User, UserDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
        CreateMap<UserProfileDto, UserProfile>().ReverseMap();

        CreateMap<ReviewAppDto, ReviewApp>().ReverseMap();
        CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));

        CreateMap<ClubDto, Club>().ReverseMap();
        CreateMap<TouristPositionDto, TouristPosition>().ReverseMap();

        CreateMap<ProfilePostDto, ProfilePost>()
            .ReverseMap();
        CreateMap<ClubPost, ClubPostDto>()
            .ForMember(dest => dest.ResourceType, opt => opt.MapFrom(src => src.ResourceType.HasValue ? (API.Dtos.ResourceTypeDto?)src.ResourceType.Value : null));
        CreateMap<ClubPostDto, ClubPost>()
            .ForMember(dest => dest.ResourceType, opt => opt.MapFrom(src => src.ResourceType.HasValue ? (Domain.ResourceType?)src.ResourceType.Value : null));

        CreateMap<Club, ClubDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<ClubStatus>(src.Status)));

        CreateMap<ClubMember, ClubMemberDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ReverseMap();
        CreateMap<ClubMembershipRequest, ClubMembershipRequestDto>().ReverseMap();
        CreateMap<ClubMember, ClubMemberDto>().ReverseMap();
    }
}
