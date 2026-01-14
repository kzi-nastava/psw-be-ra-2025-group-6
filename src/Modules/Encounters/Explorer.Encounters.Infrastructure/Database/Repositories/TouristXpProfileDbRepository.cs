using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Explorer.Encounters.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Encounters.Infrastructure.Database.Repositories
{
    public class TouristXpProfileDbRepository : ITouristXpProfileRepository
    {
        private readonly EncountersContext _context;

        public TouristXpProfileDbRepository(EncountersContext context)
        {
            _context = context;
        }

        public TouristXpProfile? GetByUserId(long userId)
        {
            return _context.TouristXpProfiles.FirstOrDefault(p => p.UserId == userId);
        }

        public TouristXpProfile Create(TouristXpProfile profile)
        {
            _context.TouristXpProfiles.Add(profile);
            _context.SaveChanges();
            return profile;
        }

        public TouristXpProfile Update(TouristXpProfile profile)
        {
            _context.TouristXpProfiles.Update(profile);
            _context.SaveChanges();
            return profile;
        }
    }
}
