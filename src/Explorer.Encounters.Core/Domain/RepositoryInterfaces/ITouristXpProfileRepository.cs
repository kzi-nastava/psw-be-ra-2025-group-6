using Explorer.Encounters.Core.Domain;

namespace Explorer.Encounters.Core.Domain.RepositoryInterfaces
{
    public interface ITouristXpProfileRepository
    {
        TouristXpProfile? GetByUserId(long userId);
        TouristXpProfile Create(TouristXpProfile profile);
        TouristXpProfile Update(TouristXpProfile profile);
    }
}
