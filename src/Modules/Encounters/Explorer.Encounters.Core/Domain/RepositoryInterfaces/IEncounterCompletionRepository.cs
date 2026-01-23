using Explorer.Encounters.Core.Domain;

namespace Explorer.Encounters.Core.Domain.RepositoryInterfaces
{
    public interface IEncounterCompletionRepository
    {
        EncounterCompletion Create(EncounterCompletion completion);
        bool HasUserCompletedChallenge(long userId, long challengeId);
        List<EncounterCompletion> GetByUserId(long userId);
    }
}
