using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Mappers;

public class StakeholderMapperProfile : Profile
{
    public StakeholderMapperProfile()
    {
        CreateMap<TourProblemDto, TourProblem>().ReverseMap();
        CreateMap<TourProblemMessageDto, TourProblemMessage>().ReverseMap();
        CreateMap<PagedResult<TourProblemMessage>, PagedResult<TourProblemMessageDto>>().ReverseMap();
        CreateMap<NotificationDto, Notification>().ReverseMap();
        CreateMap<PagedResult<Notification>, PagedResult<NotificationDto>>().ReverseMap();
        CreateMap<CreateUserDto, Person>();
    }
}
