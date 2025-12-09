using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Execution;

public interface ITourExecutionService
{
    TourExecutionStartResultDto StartExecution(TourExecutionStartDto dto, long touristId);
    TourExecutionStartResultDto? GetActiveExecution(long touristId, long? tourId = null);
    ProgressResponseDto CheckProgress(long executionId, TrackPointDto dto, long touristId);
    UnlockedSecretsDto GetUnlockedSecrets(long executionId, long touristId);
}
