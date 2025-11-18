using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
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
        try
        {
            var existingProfile = _userProfileRepository.Get(userProfileDto.UserId);
            existingProfile.Update(userProfileDto.Name, userProfileDto.Surname, userProfileDto.ProfilePicture, userProfileDto.Biography, userProfileDto.Quote);
            var updatedProfile = _userProfileRepository.Update(existingProfile);
            return _mapper.Map<UserProfileDto>(updatedProfile);
        }
        catch (NotFoundException)
        {
            // Profile does not exist, so create it
            var newProfile = _mapper.Map<UserProfile>(userProfileDto);
            var createdProfile = _userProfileRepository.Create(newProfile);
            return _mapper.Map<UserProfileDto>(createdProfile);
        }
    }
}
