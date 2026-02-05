using Explorer.Encounters.API.Dtos;

namespace Explorer.Encounters.API.Public
{
    public interface IHiddenLocationService
    {
        HiddenLocationAttemptDto StartAttempt(StartHiddenLocationDto dto, long userId);
        HiddenLocationProgressDto UpdateProgress(UpdateHiddenLocationProgressDto dto, long userId);
        HiddenLocationAttemptDto? GetActiveAttempt(long userId, long challengeId);
        List<HiddenLocationAttemptDto> GetUserAttempts(long userId, long challengeId);
        ActivationCheckDto CheckActivation(long challengeId, double userLatitude, double userLongitude);
    }
}
