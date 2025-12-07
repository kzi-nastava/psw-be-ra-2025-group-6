using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourExecutionRepository
{
    TourExecution Create(TourExecution execution);
    TourExecution? GetActiveForTourist(long touristId, long? tourId = null);
}
