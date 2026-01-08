using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Domain
{
    public class ActiveSocialParticipant : Entity
    {
        public long SocialEncounterId { get; private set; }
        public long UserId { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public DateTime ActivatedAt { get; private set; }
        public DateTime LastHeartbeat { get; private set; }

        private ActiveSocialParticipant() { }

        public ActiveSocialParticipant(long socialEncounterId, long userId, double latitude, double longitude)
        {
            if (socialEncounterId <= 0) throw new ArgumentException("Invalid SocialEncounterId.");
            if (userId <= 0) throw new ArgumentException("Invalid UserId.");
            if (latitude < -90 || latitude > 90) throw new ArgumentException("Invalid Latitude.");
            if (longitude < -180 || longitude > 180) throw new ArgumentException("Invalid Longitude.");

            SocialEncounterId = socialEncounterId;
            UserId = userId;
            Latitude = latitude;
            Longitude = longitude;
            ActivatedAt = DateTime.UtcNow;
            LastHeartbeat = DateTime.UtcNow;
        }

        public void UpdateLocation(double latitude, double longitude)
        {
            if (latitude < -90 || latitude > 90) throw new ArgumentException("Invalid Latitude.");
            if (longitude < -180 || longitude > 180) throw new ArgumentException("Invalid Longitude.");

            Latitude = latitude;
            Longitude = longitude;
            LastHeartbeat = DateTime.UtcNow;
        }

        public bool IsStale(int maxSecondsWithoutHeartbeat = 30)
        {
            return (DateTime.UtcNow - LastHeartbeat).TotalSeconds > maxSecondsWithoutHeartbeat;
        }
    }
}
