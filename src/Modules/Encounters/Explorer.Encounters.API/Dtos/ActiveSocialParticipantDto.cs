using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Dtos
{
    public class ActiveSocialParticipantDto
    {
        public long Id { get; set; }
        public long SocialEncounterId { get; set; }
        public long UserId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime ActivatedAt { get; set; }
        public DateTime LastHeartbeat { get; set; }
    }
}
