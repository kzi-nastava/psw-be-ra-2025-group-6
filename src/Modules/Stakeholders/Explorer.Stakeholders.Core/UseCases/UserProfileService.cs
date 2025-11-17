using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;

public class UserProfileService : IUserProfileService
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IMapper _mapper;

    public UserProfileService(IUserProfileRepository userProfileRepository, IMapper mapper)
    {
        _userProfileRepository = userProfileRepository;
        _mapper = mapper;
    }

    public UserProfileDto Get(long userId)
    {
        var userProfile = _userProfileRepository.Get(userId);
        return _mapper.Map<UserProfileDto>(userProfile);
    }

    public UserProfileDto Update(UserProfileDto userProfileDto)
    {
        var userProfile = _mapper.Map<UserProfile>(userProfileDto);
        var updatedProfile = _userProfileRepository.Update(userProfile);
        return _mapper.Map<UserProfileDto>(updatedProfile);
    }
}
