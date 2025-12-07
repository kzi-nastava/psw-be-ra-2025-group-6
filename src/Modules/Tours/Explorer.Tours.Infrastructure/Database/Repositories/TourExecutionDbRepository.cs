using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Explorer.Tours.Infrastructure.Database.Entities;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourExecutionDbRepository : ITourExecutionRepository
{
    private readonly ToursContext _dbContext;

    public TourExecutionDbRepository(ToursContext dbContext)
    {
        _dbContext = dbContext;
    }

    public TourExecution Create(TourExecution execution)
    {
        var entity = new TourExecutionEntity
        {
            TourId = execution.TourId,
            TouristId = execution.TouristId,
            Status = execution.Status.ToString(),
            StartTime = execution.StartTime,
            EndTime = execution.EndTime,
            InitialPositionJson = JsonSerializer.Serialize(new { execution.InitialPosition.Latitude, execution.InitialPosition.Longitude }),
            ExecutionKeyPointsJson = JsonSerializer.Serialize(execution.ExecutionKeyPoints),
            LastActivity = execution.LastActivity
        };

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        var created = new TourExecution(execution.TourId, execution.TouristId, execution.InitialPosition);
        typeof(Explorer.BuildingBlocks.Core.Domain.Entity).GetProperty("Id")?.SetValue(created, entity.Id);

        return created;
    }

    public TourExecution? GetActiveForTourist(long touristId, long? tourId = null)
    {
        var query = _dbContext.Set<TourExecutionEntity>().AsQueryable()
            .Where(e => e.TouristId == touristId && e.Status == "active");

        if (tourId.HasValue)
        {
            query = query.Where(e => e.TourId == tourId.Value);
        }

        var entity = query.FirstOrDefault();
        if (entity == null) return null;

        var pos = JsonSerializer.Deserialize<Dictionary<string,double>>(entity.InitialPositionJson);
        var tp = new TrackPoint(pos!["Latitude"], pos["Longitude"]);
        var execution = new TourExecution(entity.TourId, entity.TouristId, tp);
        typeof(Explorer.BuildingBlocks.Core.Domain.Entity).GetProperty("Id")?.SetValue(execution, entity.Id);
        return execution;
    }
}
