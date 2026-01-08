namespace Explorer.Stakeholders.API.Public
{
    public interface IClubMembershipService
    {
        bool IsMember(long userId, long clubId);
    }
}
