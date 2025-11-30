using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class TouristPositionRepository : ITouristPositionRepository
{
    private readonly StakeholdersContext _dbContext;

    public TouristPositionRepository(StakeholdersContext dbContext)
    {
        _dbContext = dbContext;
    }

    public TouristPosition CreateOrUpdate(TouristPosition position)
    {
        var existingPosition = _dbContext.TouristPositions.FirstOrDefault(p => p.TouristId == position.TouristId);
        if (existingPosition != null)
        {
            existingPosition.Update(position.Latitude, position.Longitude);
            _dbContext.TouristPositions.Update(existingPosition);
        }
        else
        {
            _dbContext.TouristPositions.Add(position);
        }
        _dbContext.SaveChanges();
        return position;
    }

    public TouristPosition? GetByTouristId(long touristId)
    {
        return _dbContext.TouristPositions.FirstOrDefault(p => p.TouristId == touristId);
    }
}
