using Explorer.Stakeholders.Core.Domain;
using System.Collections.Generic;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IClubMembershipRequestRepository
    {
        ClubMembershipRequest Create(ClubMembershipRequest request);
        void Delete(long id);
        ClubMembershipRequest Get(long id);
        List<ClubMembershipRequest> GetByClub(long clubId);
        ClubMembershipRequest? GetActive(long clubId, long touristId);
    }
}