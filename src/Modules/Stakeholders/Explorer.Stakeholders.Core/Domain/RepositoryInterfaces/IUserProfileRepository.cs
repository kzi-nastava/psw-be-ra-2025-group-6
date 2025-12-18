namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IUserProfileRepository
{
    UserProfile Get(long userId);
    UserProfile Update(UserProfile userProfile);
    UserProfile Create(UserProfile userProfile);
}
