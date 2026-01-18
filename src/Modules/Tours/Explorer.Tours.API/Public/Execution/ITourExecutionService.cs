using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Execution;

public interface ITourExecutionService
{
    TourExecutionStartResultDto StartExecution(TourExecutionStartDto dto, long touristId);
    TourExecutionStartResultDto? GetActiveExecution(long touristId, long? tourId = null);
    TourExecutionResultDto CompleteExecution(long executionId, long touristId);
    TourExecutionResultDto AbandonExecution(long executionId, long touristId);
    List<TourExecutionResultDto> GetExecutedTours(long touristId);

    List<RecentTourExecutionResultDto> GetRecentExecutedTours(long touristId);

    ProgressResponseDto CheckProgress(long executionId, TrackPointDto dto, long touristId);
    UnlockedSecretsDto GetUnlockedSecrets(long executionId, long touristId);

}
