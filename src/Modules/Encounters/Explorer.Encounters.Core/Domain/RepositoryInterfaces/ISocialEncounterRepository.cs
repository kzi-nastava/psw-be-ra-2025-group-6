using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Domain.RepositoryInterfaces
{
    public interface ISocialEncounterRepository
    {
        SocialEncounter Create(SocialEncounter encounter);
        SocialEncounter Update(SocialEncounter encounter);
        SocialEncounter? Get(long id);
        SocialEncounter? GetByChallengeId(long challengeId);
        List<SocialEncounter> GetAll();
        void Delete(long id);
    }
}
