using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

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
        var userProfile = _dbContext.UserProfiles.FirstOrDefault(u => u.UserId == userId);
        if (userProfile == null) throw new NotFoundException("User profile not found: " + userId);
        return userProfile;
    }

    public UserProfile Update(UserProfile userProfile)
    {
        try
        {
            _dbContext.UserProfiles.Update(userProfile);
            _dbContext.SaveChanges();
        }
        catch (DbUpdateException e)
        {
            throw new NotFoundException(e.Message);
        }
        return userProfile;
    }
}