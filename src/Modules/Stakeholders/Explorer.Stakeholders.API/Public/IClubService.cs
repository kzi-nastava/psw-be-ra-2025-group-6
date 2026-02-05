using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface IClubService
    {
        ClubDto Create(ClubDto club);
        ClubDto Update(ClubDto club);
        void Delete(long id);
        ClubDto Get(long id);
        List<ClubDto> GetAll();
        
        // Owner controls
        ClubDto ChangeStatus(long clubId, string status, long ownerId);
        List<ClubMemberDto> GetMembers(long clubId);
        ClubMemberDto InviteMember(long clubId, string username, long ownerId);
        void RemoveMember(long clubId, long memberId, long ownerId);
    }
}
