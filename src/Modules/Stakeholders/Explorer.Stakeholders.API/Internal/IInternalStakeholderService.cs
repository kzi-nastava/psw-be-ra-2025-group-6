namespace Explorer.Stakeholders.API.Internal;
public interface IInternalStakeholderService
{
    string GetUsername(long userId);
    string GetProfilePicture(long userId);
    List<long> GetFollowedIds(long followerId);
}

