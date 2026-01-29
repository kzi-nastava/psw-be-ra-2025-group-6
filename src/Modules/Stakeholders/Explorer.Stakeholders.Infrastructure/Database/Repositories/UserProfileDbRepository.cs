using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
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

    public UserProfile Create(UserProfile userProfile)
    {
        _dbContext.UserProfiles.Add(userProfile);
        _dbContext.SaveChanges();
        return userProfile;
    }

    public List<Achievement> GetAchievements(long userId)
    {
        var profile = _dbContext.UserProfiles
            .Include(p => p.Achievements)
            .FirstOrDefault(p => p.UserId == userId);

        if (profile == null)
            throw new KeyNotFoundException("User profile not found.");

        return profile.Achievements.ToList();
    }

    public void AddAchievement(long userId, long achievementId)
    {
        var profile = _dbContext.UserProfiles
            .Include(p => p.Achievements)
            .FirstOrDefault(p => p.UserId == userId);

        if (profile == null)
            throw new NotFoundException("User profile not found.");

        // Prevent duplicates
        if (profile.Achievements.Any(a => a.Id == achievementId))
            return;

        var achievement = _dbContext.Set<Achievement>()
            .FirstOrDefault(a => a.Id == achievementId);

        if (achievement == null)
            throw new NotFoundException("Achievement not found.");

        profile.Achievements.Add(achievement);
        _dbContext.SaveChanges();
    }

}