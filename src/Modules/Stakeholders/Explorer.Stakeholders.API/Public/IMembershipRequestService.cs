using Explorer.Stakeholders.API.Dtos;
using System.Collections.Generic;

namespace Explorer.Stakeholders.API.Public
{
    public interface IMembershipRequestService
    {
        ClubMembershipRequestDto SendRequest(long clubId, long touristId);
        void WithdrawRequest(long requestId, long userId);
        List<ClubMembershipRequestDto> GetPendingRequestsByClub(long clubId, long ownerId);
        void AcceptRequest(long requestId, long ownerId);
        void RejectRequest(long requestId, long ownerId);
    }
}