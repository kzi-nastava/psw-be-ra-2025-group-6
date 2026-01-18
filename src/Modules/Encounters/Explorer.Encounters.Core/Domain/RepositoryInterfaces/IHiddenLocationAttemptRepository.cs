using Explorer.Encounters.Core.Domain;

namespace Explorer.Encounters.Core.Domain.RepositoryInterfaces
{
    public interface IHiddenLocationAttemptRepository
    {
        HiddenLocationAttempt Create(HiddenLocationAttempt attempt);
        HiddenLocationAttempt Get(long id);
        HiddenLocationAttempt? GetActiveAttempt(long userId, long challengeId);
        List<HiddenLocationAttempt> GetUserAttempts(long userId, long challengeId);
        HiddenLocationAttempt Update(HiddenLocationAttempt attempt);
    }
}
