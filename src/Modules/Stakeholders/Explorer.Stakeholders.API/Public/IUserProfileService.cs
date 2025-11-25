using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public;

public interface IUserProfileService
{
    UserProfileDto Get(long userId);
    UserProfileDto Update(UserProfileDto userProfile);
}
