using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Execution;

public interface ITourExecutionService
{
    TourExecutionStartResultDto StartExecution(TourExecutionStartDto dto, long touristId);
    TourExecutionStartResultDto? GetActiveExecution(long touristId, long? tourId = null);
    TourExecutionResultDto CompleteExecution(long executionId, long touristId);
    TourExecutionResultDto AbandonExecution(long executionId, long touristId);
}
