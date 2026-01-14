using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Domain.RepositoryInterfaces
{
    public interface IActiveSocialParticipantRepository
    {
        ActiveSocialParticipant Create(ActiveSocialParticipant participant);
        ActiveSocialParticipant Update(ActiveSocialParticipant participant);
        ActiveSocialParticipant? Get(long id);
        ActiveSocialParticipant? GetByUserAndEncounter(long userId, long socialEncounterId);
        List<ActiveSocialParticipant> GetAllActiveForEncounter(long socialEncounterId);
        void Delete(long id);
        void DeleteByUserAndEncounter(long userId, long socialEncounterId);
    }
}
