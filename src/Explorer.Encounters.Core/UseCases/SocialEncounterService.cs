using AutoMapper;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.UseCases
{
    public class SocialEncounterService : ISocialEncounterService
    {
        private readonly ISocialEncounterRepository _socialEncounterRepository;
        private readonly IActiveSocialParticipantRepository _participantRepository;
        private readonly IChallengeRepository _challengeRepository;
        private readonly ITouristXpProfileRepository _profileRepository;
        private readonly IEncounterCompletionRepository _completionRepository;
        private readonly IMapper _mapper;

        // Koliko sekundi čekamo da korisnik pošalje heartbeat pre nego što ga smatramo neaktivnim
        private const int MAX_SECONDS_WITHOUT_HEARTBEAT = 30;

        public SocialEncounterService(
            ISocialEncounterRepository socialEncounterRepository,
            IActiveSocialParticipantRepository participantRepository,
            IChallengeRepository challengeRepository,
            ITouristXpProfileRepository profileRepository,
            IEncounterCompletionRepository completionRepository,
            IMapper mapper)
        {
            _socialEncounterRepository = socialEncounterRepository;
            _participantRepository = participantRepository;
            _challengeRepository = challengeRepository;
            _profileRepository = profileRepository;
            _completionRepository = completionRepository;
            _mapper = mapper;
        }

        #region Admin Methods

        public SocialEncounterDto CreateSocialEncounter(SocialEncounterDto dto)
        {
            // Provera da li Challenge postoji
            var challenge = _challengeRepository.Get(dto.ChallengeId);
            if (challenge == null)
                throw new KeyNotFoundException("Challenge not found.");

            // Provera da li je Challenge tipa Social
            if (challenge.Type != ChallengeType.Social)
                throw new InvalidOperationException("Challenge must be of type Social.");

            // Provera da li već postoji Social Encounter za ovaj Challenge
            var existing = _socialEncounterRepository.GetByChallengeId(dto.ChallengeId);
            if (existing != null)
                throw new InvalidOperationException("Social Encounter already exists for this Challenge.");

            var socialEncounter = new SocialEncounter(dto.ChallengeId, dto.RequiredPeople, dto.RadiusMeters);
            var created = _socialEncounterRepository.Create(socialEncounter);

            return _mapper.Map<SocialEncounterDto>(created);
        }

        public SocialEncounterDto UpdateSocialEncounter(long id, SocialEncounterDto dto)
        {
            var existing = _socialEncounterRepository.Get(id);
            if (existing == null)
                throw new KeyNotFoundException("Social Encounter not found.");

            existing.Update(dto.RequiredPeople, dto.RadiusMeters);
            var updated = _socialEncounterRepository.Update(existing);

            return _mapper.Map<SocialEncounterDto>(updated);
        }

        public SocialEncounterDto GetSocialEncounter(long id)
        {
            var socialEncounter = _socialEncounterRepository.Get(id);
            return _mapper.Map<SocialEncounterDto>(socialEncounter);
        }

        public SocialEncounterDto GetByChallengeId(long challengeId)
        {
            var socialEncounter = _socialEncounterRepository.GetByChallengeId(challengeId);
            return _mapper.Map<SocialEncounterDto>(socialEncounter);
        }

        public void DeleteSocialEncounter(long id)
        {
            _socialEncounterRepository.Delete(id);
        }

        #endregion

        #region Tourist Methods

        public ActivateSocialEncounterResponseDto ActivateSocialEncounter(
            long challengeId,
            long userId,
            ActivateSocialEncounterRequestDto request)
        {
            // 1. Provera da li Challenge postoji i da je Active
            var challenge = _challengeRepository.Get(challengeId);
            if (challenge == null)
            {
                return new ActivateSocialEncounterResponseDto
                {
                    Success = false,
                    Message = "Challenge not found."
                };
            }

            if (challenge.Status != ChallengeStatus.Active)
            {
                return new ActivateSocialEncounterResponseDto
                {
                    Success = false,
                    Message = "Challenge is not active."
                };
            }

            // 2. Provera da li je Challenge tipa Social
            if (challenge.Type != ChallengeType.Social)
            {
                return new ActivateSocialEncounterResponseDto
                {
                    Success = false,
                    Message = "This challenge is not a Social Encounter."
                };
            }

            // 3. Provera da li turista već ima completed ovaj challenge
            if (_completionRepository.HasUserCompletedChallenge(userId, challengeId))
            {
                return new ActivateSocialEncounterResponseDto
                {
                    Success = false,
                    Message = "You have already completed this challenge."
                };
            }

            // 4. Nalaženje Social Encounter konfiguracije
            var socialEncounter = _socialEncounterRepository.GetByChallengeId(challengeId);
            if (socialEncounter == null)
            {
                return new ActivateSocialEncounterResponseDto
                {
                    Success = false,
                    Message = "Social Encounter configuration not found."
                };
            }

            // 5. Provera da li je turista u radijusu
            var distance = CalculateDistance(
                request.CurrentLatitude,
                request.CurrentLongitude,
                challenge.Latitude,
                challenge.Longitude
            );

            if (distance > socialEncounter.RadiusMeters)
            {
                return new ActivateSocialEncounterResponseDto
                {
                    Success = false,
                    Message = $"You are too far from the encounter location. You need to be within {socialEncounter.RadiusMeters}m.",
                    IsWithinRadius = false,
                    CurrentActiveCount = 0,
                    RequiredPeople = socialEncounter.RequiredPeople
                };
            }

            // 6. Provera da li je turista već aktivan u ovom encounter-u
            var existingParticipant = _participantRepository.GetByUserAndEncounter(userId, socialEncounter.Id);
            if (existingParticipant != null)
            {
                // Već je aktivan, samo update-ujemo lokaciju
                existingParticipant.UpdateLocation(request.CurrentLatitude, request.CurrentLongitude);
                _participantRepository.Update(existingParticipant);
            }
            else
            {
                // Dodajemo novog participanta
                var newParticipant = new ActiveSocialParticipant(
                    socialEncounter.Id,
                    userId,
                    request.CurrentLatitude,
                    request.CurrentLongitude
                );
                _participantRepository.Create(newParticipant);
            }

            // 7. Čistimo stare (neaktivne) participante
            CleanupStaleParticipants(socialEncounter.Id);

            // 8. Brojimo aktivne participante koji su u radijusu
            var activeCount = CountActiveParticipantsInRadius(socialEncounter.Id, challenge.Latitude, challenge.Longitude, socialEncounter.RadiusMeters);

            return new ActivateSocialEncounterResponseDto
            {
                Success = true,
                Message = $"Social Encounter activated! {activeCount}/{socialEncounter.RequiredPeople} people are here.",
                IsWithinRadius = true,
                CurrentActiveCount = activeCount,
                RequiredPeople = socialEncounter.RequiredPeople
            };
        }

        public SocialEncounterHeartbeatResponseDto SendHeartbeat(
            long challengeId,
            long userId,
            SocialEncounterHeartbeatRequestDto request)
        {
            // 1. Provera da li Challenge postoji
            var challenge = _challengeRepository.Get(challengeId);
            if (challenge == null)
            {
                return new SocialEncounterHeartbeatResponseDto
                {
                    Success = false,
                    Message = "Challenge not found."
                };
            }

            // 2. Nalaženje Social Encounter konfiguracije
            var socialEncounter = _socialEncounterRepository.GetByChallengeId(challengeId);
            if (socialEncounter == null)
            {
                return new SocialEncounterHeartbeatResponseDto
                {
                    Success = false,
                    Message = "Social Encounter configuration not found."
                };
            }

            // 3. Provera da li je korisnik već completed ovaj challenge
            if (_completionRepository.HasUserCompletedChallenge(userId, challengeId))
            {
                return new SocialEncounterHeartbeatResponseDto
                {
                    Success = false,
                    Message = "You have already completed this challenge."
                };
            }

            // 4. Nalaženje participanta
            var participant = _participantRepository.GetByUserAndEncounter(userId, socialEncounter.Id);
            if (participant == null)
            {
                return new SocialEncounterHeartbeatResponseDto
                {
                    Success = false,
                    Message = "You are not active in this encounter. Please activate it first."
                };
            }

            // 5. Provera da li je turista i dalje u radijusu
            var distance = CalculateDistance(
                request.CurrentLatitude,
                request.CurrentLongitude,
                challenge.Latitude,
                challenge.Longitude
            );

            bool isInRadius = distance <= socialEncounter.RadiusMeters;

            if (!isInRadius)
            {
                // Izašao je iz radijusa - deaktiviramo ga
                _participantRepository.Delete(participant.Id);

                return new SocialEncounterHeartbeatResponseDto
                {
                    Success = true,
                    Message = "You left the encounter area.",
                    StillInRadius = false,
                    CurrentActiveCount = CountActiveParticipantsInRadius(socialEncounter.Id, challenge.Latitude, challenge.Longitude, socialEncounter.RadiusMeters),
                    RequiredPeople = socialEncounter.RequiredPeople,
                    IsCompleted = false
                };
            }

            // 6. Update-ujemo lokaciju i heartbeat
            participant.UpdateLocation(request.CurrentLatitude, request.CurrentLongitude);
            _participantRepository.Update(participant);

            // 7. Čistimo stare participante
            CleanupStaleParticipants(socialEncounter.Id);

            // 8. Brojimo aktivne participante u radijusu
            var activeCount = CountActiveParticipantsInRadius(socialEncounter.Id, challenge.Latitude, challenge.Longitude, socialEncounter.RadiusMeters);

            // 9. Provera da li je encounter completed (dovoljno ljudi)
            if (activeCount >= socialEncounter.RequiredPeople)
            {
                // ENCOUNTER JE ZAVRŠEN! 🎉
                return CompleteEncounterForAllParticipants(socialEncounter.Id, challenge, activeCount);
            }

            // Još uvek nedovoljno ljudi
            return new SocialEncounterHeartbeatResponseDto
            {
                Success = true,
                Message = $"Still waiting... {activeCount}/{socialEncounter.RequiredPeople} people are here.",
                StillInRadius = true,
                CurrentActiveCount = activeCount,
                RequiredPeople = socialEncounter.RequiredPeople,
                IsCompleted = false
            };
        }

        public DeactivateSocialEncounterResponseDto DeactivateSocialEncounter(long challengeId, long userId)
        {
            var socialEncounter = _socialEncounterRepository.GetByChallengeId(challengeId);
            if (socialEncounter == null)
            {
                return new DeactivateSocialEncounterResponseDto
                {
                    Success = false,
                    Message = "Social Encounter not found."
                };
            }

            var participant = _participantRepository.GetByUserAndEncounter(userId, socialEncounter.Id);
            if (participant == null)
            {
                return new DeactivateSocialEncounterResponseDto
                {
                    Success = false,
                    Message = "You are not active in this encounter."
                };
            }

            _participantRepository.Delete(participant.Id);

            return new DeactivateSocialEncounterResponseDto
            {
                Success = true,
                Message = "You have left the Social Encounter."
            };
        }

        #endregion

        #region Helper Methods

        private void CleanupStaleParticipants(long socialEncounterId)
        {
            var allParticipants = _participantRepository.GetAllActiveForEncounter(socialEncounterId);

            foreach (var participant in allParticipants)
            {
                if (participant.IsStale(MAX_SECONDS_WITHOUT_HEARTBEAT))
                {
                    _participantRepository.Delete(participant.Id);
                }
            }
        }

        private int CountActiveParticipantsInRadius(long socialEncounterId, double centerLat, double centerLon, double radiusMeters)
        {
            var allParticipants = _participantRepository.GetAllActiveForEncounter(socialEncounterId);

            int count = 0;
            foreach (var participant in allParticipants)
            {
                if (participant.IsStale(MAX_SECONDS_WITHOUT_HEARTBEAT))
                    continue;

                var distance = CalculateDistance(
                    participant.Latitude,
                    participant.Longitude,
                    centerLat,
                    centerLon
                );

                if (distance <= radiusMeters)
                    count++;
            }

            return count;
        }

        private SocialEncounterHeartbeatResponseDto CompleteEncounterForAllParticipants(
            long socialEncounterId,
            Challenge challenge,
            int activeCount)
        {
            var allParticipants = _participantRepository.GetAllActiveForEncounter(socialEncounterId);

            // Dodeljujemo XP svim aktivnim participantima koji nisu već completed
            foreach (var participant in allParticipants)
            {
                if (participant.IsStale(MAX_SECONDS_WITHOUT_HEARTBEAT))
                    continue;

                // Provera da nije već completed
                if (_completionRepository.HasUserCompletedChallenge(participant.UserId, challenge.Id))
                    continue;

                // Get or create profile
                var profile = _profileRepository.GetByUserId(participant.UserId);
                if (profile == null)
                {
                    profile = new TouristXpProfile(participant.UserId);
                    profile = _profileRepository.Create(profile);
                }

                // Dodela XP
                profile.AddXP(challenge.XP);
                _profileRepository.Update(profile);

                // Zabeležavanje completion
                var completion = new EncounterCompletion(participant.UserId, challenge.Id, challenge.XP);
                _completionRepository.Create(completion);

                // Brisanje participanta (encounter završen za njega)
                _participantRepository.Delete(participant.Id);
            }

            return new SocialEncounterHeartbeatResponseDto
            {
                Success = true,
                Message = $"🎉 Social Encounter completed! You earned {challenge.XP} XP!",
                StillInRadius = true,
                CurrentActiveCount = activeCount,
                RequiredPeople = activeCount,
                IsCompleted = true,
                XpAwarded = challenge.XP
            };
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000; // Earth radius in meters
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        #endregion
    }
}
