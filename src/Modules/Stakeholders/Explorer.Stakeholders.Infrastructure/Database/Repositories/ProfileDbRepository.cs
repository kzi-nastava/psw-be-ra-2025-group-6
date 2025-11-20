using System.Linq;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class ProfileDbRepository : IProfileRepository
{
    private readonly StakeholdersContext _dbContext;

    public ProfileDbRepository(StakeholdersContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Profile? GetByPersonId(long personId)
    {
        return _dbContext.Profiles.FirstOrDefault(p => p.PersonId == personId);
    }

    public Profile Create(Profile entity)
    {
        _dbContext.Profiles.Add(entity);
        _dbContext.SaveChanges();
        return entity;
    }

    public Profile Update(Profile entity)
    {
        _dbContext.Profiles.Update(entity);
        _dbContext.SaveChanges();
        return entity;
    }
}
