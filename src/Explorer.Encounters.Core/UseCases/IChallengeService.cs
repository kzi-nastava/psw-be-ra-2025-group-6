using Explorer.Encounters.API.Dtos;
using System.Collections.Generic;

namespace Explorer.Encounters.Core.UseCases
{
    public interface IChallengeService
    {
        List<ChallengeDto> GetActive();
        List<ChallengeDto> GetAll();
        ChallengeDto Create(ChallengeDto dto);
        ChallengeDto Update(long id, ChallengeDto dto);
        void Delete(long id);
        ChallengeDto Get(long id);
    }
}
