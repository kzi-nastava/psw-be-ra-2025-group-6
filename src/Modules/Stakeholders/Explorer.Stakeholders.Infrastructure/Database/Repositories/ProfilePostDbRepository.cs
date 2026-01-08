using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class ProfilePostDbRepository : CrudDatabaseRepository<ProfilePost, StakeholdersContext>, IProfilePostRepository
{
    private readonly StakeholdersContext _dbContext;

    public ProfilePostDbRepository(StakeholdersContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public List<ProfilePost> GetByAuthor(long authorId)
    {
        return _dbContext.ProfilePosts
            .AsNoTracking()
            .Where(p => p.AuthorId == authorId)
            .OrderByDescending(p => p.CreatedAt)
            .ToList();
    }
}
