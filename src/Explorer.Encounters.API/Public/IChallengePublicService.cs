using Explorer.Encounters.API.Dtos;
using System.Collections.Generic;

namespace Explorer.Encounters.API.Public
{
    public interface IChallengePublicService
    {
        List<ChallengeDto> GetActive();
        ChallengeDto Get(long id);
    }
}
