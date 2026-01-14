using Explorer.Encounters.API.Dtos;

namespace Explorer.Encounters.Core.UseCases
{
    public interface ITouristEncounterService
    {
        TouristXpProfileDto GetOrCreateProfile(long userId);
        ActiveChallengesResponseDto GetActiveChallenges(long userId);
        CompleteEncounterResponseDto CompleteEncounter(long userId, CompleteEncounterRequestDto request);
        List<EncounterCompletionDto> GetCompletedEncounters(long userId);
    }
}
