using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public
{
    public interface ITourCheckpointPlanService
    {
        TourCheckpointPlanDto Create(long userId, TourCheckpointPlanCreateDto dto);
        TourCheckpointPlanDto Update(long id, long userId, TourCheckpointPlanUpdateDto dto);
        void Delete(long id, long userId);
        TourCheckpointPlanDto GetById(long id, long userId);
        List<TourCheckpointPlanDto> GetByPlannerItemId(long plannerItemId, long userId);
    }
}
