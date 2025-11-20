using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public;

public interface IProfileService
{
    ProfileDto GetProfile(long personId);
    ProfileDto UpdateProfile(long personId, ProfileDto profile);
}
