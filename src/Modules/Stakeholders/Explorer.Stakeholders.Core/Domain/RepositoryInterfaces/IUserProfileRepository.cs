using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IUserProfileRepository
{
    UserProfile Get(long userId);
    UserProfile Update(UserProfile userProfile);
    UserProfile Create(UserProfile userProfile);
    List<UserProfile> GetAll();
    List<Achievement> GetAchievements(long userId);

    public void AddAchievement(long userId, long achievementId);
}
