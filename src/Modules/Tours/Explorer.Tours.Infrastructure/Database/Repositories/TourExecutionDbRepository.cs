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
            LastActivity = execution.LastActivity,
            CompletedKeyPointsJson = JsonSerializer.Serialize(execution.CompletedKeyPoints.Select(ckp => new
            {
                ckp.KeyPointId,
                ckp.KeyPointName,
                ckp.CompletedAt,
                ckp.UnlockedSecret
            })),
            ProgressPercentage = execution.ProgressPercentage,
            CurrentKeyPointId = execution.CurrentKeyPointId
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

        return MapToDomain(entity);
    }

    public List<TourExecution> GetAll(long touristId)
    {
        var entities = _dbContext.Set<TourExecutionEntity>().AsNoTracking()
            .Where(e => e.TouristId == touristId)
            .ToList();

        return entities.Select(MapToDomain).ToList();
    }

    public TourExecution? GetById(long executionId)
    {
        var entity = _dbContext.Set<TourExecutionEntity>().Find(executionId);
        if (entity == null) return null;

        return MapToDomain(entity);
    }

    public TourExecution Get(long id)
    {
        var entity = _dbContext.Set<TourExecutionEntity>().FirstOrDefault(e => e.Id == id);
        if (entity == null) throw new NotFoundException($"Tour execution with id {id} not found");

        return MapToDomain(entity);
    }

    public TourExecution Update(TourExecution execution)
    {
        var entity = _dbContext.Set<TourExecutionEntity>().FirstOrDefault(e => e.Id == execution.Id);
        if (entity == null) throw new NotFoundException($"Tour execution with id {execution.Id} not found");

        entity.Status = execution.Status.ToString();
        entity.EndTime = execution.EndTime;
        entity.ExecutionKeyPointsJson = JsonSerializer.Serialize(execution.ExecutionKeyPoints);
        entity.LastActivity = execution.LastActivity;
        entity.CompletedKeyPointsJson = JsonSerializer.Serialize(execution.CompletedKeyPoints.Select(ckp => new
        {
            ckp.KeyPointId,
            ckp.KeyPointName,
            ckp.CompletedAt,
            ckp.UnlockedSecret
        }));
        entity.ProgressPercentage = execution.ProgressPercentage;
        entity.CurrentKeyPointId = execution.CurrentKeyPointId;

        _dbContext.SaveChanges();

        return MapToDomain(entity);
    }

    private TourExecution MapToDomain(TourExecutionEntity entity)
    {
        var pos = JsonSerializer.Deserialize<Dictionary<string, double>>(entity.InitialPositionJson);
        var tp = new TrackPoint(pos!["Latitude"], pos["Longitude"]);
        var execution = new TourExecution(entity.TourId, entity.TouristId, tp);
        typeof(Explorer.BuildingBlocks.Core.Domain.Entity).GetProperty("Id")?.SetValue(execution, entity.Id);

        // Restore StartTime from entity
        typeof(TourExecution).GetProperty("StartTime")?.SetValue(execution, entity.StartTime);

        var executionKeyPoints = JsonSerializer.Deserialize<List<long>>(entity.ExecutionKeyPointsJson) ?? new List<long>();
        foreach (var kpId in executionKeyPoints)
        {
            execution.AddExecutionKeyPoint(kpId);
        }

        if (!string.IsNullOrEmpty(entity.CompletedKeyPointsJson))
        {
            var completedKpData = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(entity.CompletedKeyPointsJson);
            if (completedKpData != null)
            {
                foreach (var ckp in completedKpData)
                {
                    var keyPointId = ckp["KeyPointId"].GetInt64();
                    var keyPointName = ckp["KeyPointName"].GetString();
                    var unlockedSecret = ckp["UnlockedSecret"].GetString();

                    if (!string.IsNullOrEmpty(keyPointName) && !string.IsNullOrEmpty(unlockedSecret))
                    {
                        execution.CompleteKeyPoint(keyPointId, keyPointName, unlockedSecret);
                    }
                }
            }
        }

        // Update progress without changing LastActivity
        typeof(TourExecution).GetProperty("ProgressPercentage")?.SetValue(execution, entity.ProgressPercentage);
        typeof(TourExecution).GetProperty("CurrentKeyPointId")?.SetValue(execution, entity.CurrentKeyPointId);
        
        // Restore LastActivity from entity (instead of setting it to Now)
        typeof(TourExecution).GetProperty("LastActivity")?.SetValue(execution, entity.LastActivity);

        if (entity.Status == "completed")
        {
            execution.Complete();
        }
        else if (entity.Status == "abandoned")
        {
            execution.Abandon();
        }

        return execution;
    }
}
