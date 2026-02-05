using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourCheckpointPlanDbRepository : ITourCheckpointPlanRepository
{
    private readonly ToursContext _dbContext;
    private readonly DbSet<TourCheckpointPlan> _plans;

    public TourCheckpointPlanDbRepository(ToursContext dbContext)
    {
        _dbContext = dbContext;
        _plans = _dbContext.Set<TourCheckpointPlan>();
    }

    public TourCheckpointPlan GetById(long id)
    {
        var plan = _plans.FirstOrDefault(p => p.Id == id);
        if (plan == null) throw new NotFoundException("Tour checkpoint plan not found: " + id);
        return plan;
    }

    public List<TourCheckpointPlan> GetByPlannerItemId(long plannerItemId)
    {
        return _plans.Where(p => p.PlannerItemId == plannerItemId)
            .OrderBy(p => p.PlannedAt)
            .ToList();
    }

    public TourCheckpointPlan Create(TourCheckpointPlan entity)
    {
        _plans.Add(entity);
        _dbContext.SaveChanges();
        return entity;
    }

    public void Update(TourCheckpointPlan entity)
    {
        _plans.Update(entity);
        _dbContext.SaveChanges();
    }

    public void Delete(long id)
    {
        var plan = _plans.FirstOrDefault(p => p.Id == id);
        if (plan == null) throw new NotFoundException("Tour checkpoint plan not found: " + id);
        _plans.Remove(plan);
        _dbContext.SaveChanges();
    }
}
