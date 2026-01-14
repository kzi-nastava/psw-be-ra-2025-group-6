using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Infrastructure.Database.Repositories
{
    public class ActiveSocialParticipantDatabaseRepository : IActiveSocialParticipantRepository
    {
        private readonly EncountersContext _dbContext;

        public ActiveSocialParticipantDatabaseRepository(EncountersContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ActiveSocialParticipant Create(ActiveSocialParticipant participant)
        {
            _dbContext.ActiveSocialParticipants.Add(participant);
            _dbContext.SaveChanges();
            return participant;
        }

        public ActiveSocialParticipant Update(ActiveSocialParticipant participant)
        {
            _dbContext.ActiveSocialParticipants.Update(participant);
            _dbContext.SaveChanges();
            return participant;
        }

        public ActiveSocialParticipant? Get(long id)
        {
            return _dbContext.ActiveSocialParticipants.Find(id);
        }

        public ActiveSocialParticipant? GetByUserAndEncounter(long userId, long socialEncounterId)
        {
            return _dbContext.ActiveSocialParticipants
                .FirstOrDefault(p => p.UserId == userId && p.SocialEncounterId == socialEncounterId);
        }

        public List<ActiveSocialParticipant> GetAllActiveForEncounter(long socialEncounterId)
        {
            return _dbContext.ActiveSocialParticipants
                .Where(p => p.SocialEncounterId == socialEncounterId)
                .ToList();
        }

        public void Delete(long id)
        {
            var participant = Get(id);
            if (participant != null)
            {
                _dbContext.ActiveSocialParticipants.Remove(participant);
                _dbContext.SaveChanges();
            }
        }

        public void DeleteByUserAndEncounter(long userId, long socialEncounterId)
        {
            var participant = GetByUserAndEncounter(userId, socialEncounterId);
            if (participant != null)
            {
                _dbContext.ActiveSocialParticipants.Remove(participant);
                _dbContext.SaveChanges();
            }
        }
    }
}
