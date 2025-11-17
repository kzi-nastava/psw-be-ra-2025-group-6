using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class UserProfileDbRepository : IUserProfileRepository
{
    private readonly StakeholdersContext _dbContext;

    public UserProfileDbRepository(StakeholdersContext dbContext)
    {
        _dbContext = dbContext;
    }

    public UserProfile Get(long userId)
    {
        // Demo
        return new UserProfile(userId, "Test", "User", "test.jpg", "test bio", "test quote");
    }

    public UserProfile Update(UserProfile userProfile)
    {
        // Demo
        _dbContext.Update(userProfile);
        _dbContext.SaveChanges();
        return userProfile;
    }
}
