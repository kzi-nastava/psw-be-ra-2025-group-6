using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Domain
{
    public class SocialEncounter : AggregateRoot
    {
        public long ChallengeId { get; private set; }
        public int RequiredPeople { get; private set; }
        public double RadiusMeters { get; private set; }

        private SocialEncounter() { }

        public SocialEncounter(long challengeId, int requiredPeople, double radiusMeters)
        {
            if (challengeId <= 0) throw new ArgumentException("Invalid ChallengeId.");
            if (requiredPeople <= 0) throw new ArgumentException("Required people must be positive.");
            if (radiusMeters <= 0) throw new ArgumentException("Radius must be positive.");

            ChallengeId = challengeId;
            RequiredPeople = requiredPeople;
            RadiusMeters = radiusMeters;
        }

        public void Update(int requiredPeople, double radiusMeters)
        {
            if (requiredPeople <= 0) throw new ArgumentException("Required people must be positive.");
            if (radiusMeters <= 0) throw new ArgumentException("Radius must be positive.");

            RequiredPeople = requiredPeople;
            RadiusMeters = radiusMeters;
        }
    }
}
