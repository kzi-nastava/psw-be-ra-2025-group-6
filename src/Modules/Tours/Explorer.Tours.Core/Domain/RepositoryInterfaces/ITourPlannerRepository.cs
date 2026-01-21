using Explorer.BuildingBlocks.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface ITourPlannerRepository
    {
        List<TourPlanner> GetAllByUserId(long userId);
        PagedResult<TourPlanner> GetByUserId(long userId, int page, int pageSize);
        TourPlanner GetById(long id);
        bool HasOverlappingPlan(long userId, long tourId, DateTime startDate, DateTime endDate, long? excludeId = null);

        void Delete(long Id);
        TourPlanner Create(TourPlanner entity);

        void Update(TourPlanner entity);
    }
}
