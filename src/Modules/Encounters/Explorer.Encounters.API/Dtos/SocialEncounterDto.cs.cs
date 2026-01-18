using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Dtos
{
    public class SocialEncounterDto
    {
        public long Id { get; set; }
        public long ChallengeId { get; set; }
        public int RequiredPeople { get; set; }
        public double RadiusMeters { get; set; }
    }

    public class ActivateSocialEncounterRequestDto
    {
        public double CurrentLatitude { get; set; }
        public double CurrentLongitude { get; set; }
    }

    public class ActivateSocialEncounterResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int CurrentActiveCount { get; set; }
        public int RequiredPeople { get; set; }
        public bool IsWithinRadius { get; set; }
        public bool AlreadyCompleted { get; set; }
        public bool IsCompleted { get; set; }
        public int? XpAwarded { get; set; }
    }

    public class SocialEncounterHeartbeatRequestDto
    {
        public double CurrentLatitude { get; set; }
        public double CurrentLongitude { get; set; }
    }

    public class SocialEncounterHeartbeatResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int CurrentActiveCount { get; set; }
        public int RequiredPeople { get; set; }
        public bool IsCompleted { get; set; }
        public bool StillInRadius { get; set; }
        public int? XpAwarded { get; set; }
        public TouristXpProfileDto? Profile { get; set; }
    }

    public class DeactivateSocialEncounterResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}