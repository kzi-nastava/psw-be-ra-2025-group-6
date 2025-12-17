using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface IClubMemberService
    {
        List<ClubMemberDto> GetAllClubMembers(long clubId);
    }
}
