using System.Collections.Generic;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IClubRepository
    {
        Club Create(Club club);
        Club Update(Club club);
        void Delete(long id);
        Club Get(long id);
        List<Club> GetAll();
    }
}
