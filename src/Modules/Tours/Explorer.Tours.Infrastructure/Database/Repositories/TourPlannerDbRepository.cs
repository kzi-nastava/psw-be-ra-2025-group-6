using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class TourPlannerDbRepository : ITourPlannerRepository
    {
        private readonly ToursContext _dbContext;
        private readonly DbSet<TourPlanner> _planners;

        public TourPlannerDbRepository(ToursContext dbContext)
        {
            _dbContext = dbContext;
            _planners = _dbContext.Set<TourPlanner>();
        }

        public List<TourPlanner> GetAllByUserId(long userId)
        {
            return _planners
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.StartDate)
                .ToList();
        }

        public PagedResult<TourPlanner> GetByUserId(long userId, int page, int pageSize)
        {
            var query = _planners
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.StartDate);

            var totalCount = query.Count();
            var items = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<TourPlanner>(items, totalCount);
        }

        public TourPlanner GetById(long id)
        {
            var planner = _planners.FirstOrDefault(p => p.Id == id);
            if (planner == null) throw new NotFoundException("Tour planner not found: " + id);
            return planner;
        }

        public bool HasOverlappingPlan(long userId, long tourId, DateTime startDate, DateTime endDate, long? excludeId = null)
        {
            var query = _planners.Where(p =>
                p.UserId == userId &&
                p.TourId == tourId &&
                startDate <= p.EndDate &&
                endDate >= p.StartDate);

            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return query.Any();
        }

        public void Delete(long id)
        {
            var planner = _planners.FirstOrDefault(p => p.Id == id);
            if (planner == null) throw new NotFoundException("Tour planner not found: " + id);
            _planners.Remove(planner);
            _dbContext.SaveChanges();
        }

        public TourPlanner Create(TourPlanner entity)
        {
            _planners.Add(entity);
            _dbContext.SaveChanges();
            return entity;
        }

        public void Update(TourPlanner entity)
        {
            _planners.Update(entity);
            _dbContext.SaveChanges();
        }
    }
}
