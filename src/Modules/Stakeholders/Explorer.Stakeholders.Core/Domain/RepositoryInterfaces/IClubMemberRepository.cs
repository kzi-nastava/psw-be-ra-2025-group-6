using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IClubMemberRepository
    {
        ClubMember Create(ClubMember member);
        ClubMember Get(long id);
        void Delete(long id);
        ClubMember? GetByClubAndUser(long clubId, long userId);
        List<ClubMember> GetByClubId(long clubId);
        List<ClubMember> GetByUserId(long userId);
        bool IsMember(long clubId, long userId);
    }
}
