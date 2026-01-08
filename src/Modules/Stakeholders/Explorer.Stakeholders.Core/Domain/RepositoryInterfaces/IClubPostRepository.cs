using Explorer.Stakeholders.Core.Domain;
using System.Collections.Generic;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IClubPostRepository
    {
        ClubPost Create(ClubPost post);
        ClubPost Update(ClubPost post);
        void Delete(long id);
        ClubPost Get(long id);
        List<ClubPost> GetAllForClub(long clubId);
    }
}
