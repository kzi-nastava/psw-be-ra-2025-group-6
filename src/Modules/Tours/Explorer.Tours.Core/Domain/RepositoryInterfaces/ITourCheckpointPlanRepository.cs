namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourCheckpointPlanRepository
{
    TourCheckpointPlan GetById(long id);
    List<TourCheckpointPlan> GetByPlannerItemId(long plannerItemId);
    TourCheckpointPlan Create(TourCheckpointPlan entity);
    void Update(TourCheckpointPlan entity);
    void Delete(long id);
}
