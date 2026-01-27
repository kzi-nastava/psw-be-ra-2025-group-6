using System.Collections.Generic;

namespace Explorer.Encounters.Core.Domain.RepositoryInterfaces
{
    public interface IChallengeRepository
    {
        List<Challenge> GetAllActive();
        List<Challenge> GetAll();
        Challenge Get(long id);
        Challenge Create(Challenge challenge);
        Challenge Update(Challenge challenge);
        void Delete(long id);
        List<Challenge> GetPendingApproval();
    }
}
