using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;
using DomainProfile = Explorer.Stakeholders.Core.Domain.Profile;

namespace Explorer.Stakeholders.Core.Mappers;

public class StakeholderProfile : AutoMapper.Profile
{
    public StakeholderProfile()
    {
        CreateMap<DomainProfile, ProfileDto>().ReverseMap();
    }
}
