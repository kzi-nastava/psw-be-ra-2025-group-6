namespace Explorer.Stakeholders.API.Internal;
public interface IInternalStakeholderService
{
    string GetUsername(long userId);
    string GetProfilePicture(long userId);
    List<long> GetFollowedIds(long followerId);
    List<long> GetAllTouristIds();
    List<(long PersonId, string Username)> GetAllTouristsWithPersonIds();
    List<(long ClubId, string ClubName)> GetAllClubs();
}

