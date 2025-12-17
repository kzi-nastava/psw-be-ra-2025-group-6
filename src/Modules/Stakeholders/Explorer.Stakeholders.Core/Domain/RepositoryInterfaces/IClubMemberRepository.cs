namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IClubMemberRepository
    {
        List<ClubMember> GetAll(long ClubId);

    }
}
