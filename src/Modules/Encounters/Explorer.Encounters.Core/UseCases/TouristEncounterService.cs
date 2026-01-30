using AutoMapper;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Explorer.Encounters.API.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Encounters.Core.UseCases
{
    public class TouristEncounterService : ITouristEncounterService
    {
        private readonly ITouristXpProfileRepository _profileRepository;
        private readonly IEncounterCompletionRepository _completionRepository;
        private readonly IChallengeRepository _challengeRepository;
        private readonly IInternalLeaderboardService _leaderboardService;
        private readonly IMapper _mapper;

        // Proximity radius in meters for activating challenges
        private const double PROXIMITY_RADIUS_METERS = 50;

        public TouristEncounterService(
            ITouristXpProfileRepository profileRepository,
            IEncounterCompletionRepository completionRepository,
            IChallengeRepository challengeRepository,
            IInternalLeaderboardService leaderboardService,
            IMapper mapper)
        {
            _profileRepository = profileRepository;
            _completionRepository = completionRepository;
            _challengeRepository = challengeRepository;
            _leaderboardService = leaderboardService;
            _mapper = mapper;
        }

        public TouristXpProfileDto GetOrCreateProfile(long userId)
        {
            var profile = _profileRepository.GetByUserId(userId);
            
            if (profile == null)
            {
                profile = new TouristXpProfile(userId);
                profile = _profileRepository.Create(profile);
            }

            return MapToProfileDto(profile);
        }

        public ActiveChallengesResponseDto GetActiveChallenges(long userId)
        {
            var activeChallenges = _challengeRepository.GetAllActive();
            var completedChallengeIds = _completionRepository.GetByUserId(userId)
                .Select(c => c.ChallengeId)
                .ToList();

            return new ActiveChallengesResponseDto
            {
                AvailableChallenges = _mapper.Map<List<ChallengeDto>>(activeChallenges),
                CompletedChallengeIds = completedChallengeIds
            };
        }

        public CompleteEncounterResponseDto CompleteEncounter(long userId, CompleteEncounterRequestDto request)
        {
            // Check if challenge exists and is active
            var challenge = _challengeRepository.Get(request.ChallengeId);
            if (challenge == null)
            {
                return new CompleteEncounterResponseDto
                {
                    Success = false,
                    Message = "Challenge not found."
                };
            }

            if (challenge.Status != ChallengeStatus.Active)
            {
                return new CompleteEncounterResponseDto
                {
                    Success = false,
                    Message = "Challenge is not active."
                };
            }

            // Check if user already completed this challenge
            if (_completionRepository.HasUserCompletedChallenge(userId, request.ChallengeId))
            {
                return new CompleteEncounterResponseDto
                {
                    Success = false,
                    Message = "You have already completed this challenge."
                };
            }

            // Check proximity to challenge location
            var distance = CalculateDistance(
                request.CurrentLatitude,
                request.CurrentLongitude,
                challenge.Latitude,
                challenge.Longitude
            );

            if (distance > PROXIMITY_RADIUS_METERS)
            {
                return new CompleteEncounterResponseDto
                {
                    Success = false,
                    Message = $"You are too far from the challenge location. You need to be within {PROXIMITY_RADIUS_METERS}m."
                };
            }

            // Get or create user profile
            var profile = _profileRepository.GetByUserId(userId);
            if (profile == null)
            {
                profile = new TouristXpProfile(userId);
                profile = _profileRepository.Create(profile);
            }

            var oldLevel = profile.Level;

            // Award XP to user
            profile.AddXP(challenge.XP);
            profile = _profileRepository.Update(profile);

            var leveledUp = profile.Level > oldLevel;

            // Record completion
            var completion = new EncounterCompletion(userId, request.ChallengeId, challenge.XP);
            _completionRepository.Create(completion);

            // ? UPDATE LEADERBOARD STATS ?
            var coinsEarned = challenge.XP / 2;
            _ = _leaderboardService.UpdateUserStatsAsync(
                userId,
                xpGained: challenge.XP,
                challengesCompleted: 1,
                toursCompleted: 0,
                coinsEarned);

            return new CompleteEncounterResponseDto
            {
                Success = true,
                Message = $"Challenge completed! You earned {challenge.XP} XP.",
                XpAwarded = challenge.XP,
                LeveledUp = leveledUp,
                NewLevel = leveledUp ? profile.Level : null,
                Profile = MapToProfileDto(profile)
            };
        }

        public List<EncounterCompletionDto> GetCompletedEncounters(long userId)
        {
            var completions = _completionRepository.GetByUserId(userId);
            return _mapper.Map<List<EncounterCompletionDto>>(completions);
        }

        private TouristXpProfileDto MapToProfileDto(TouristXpProfile profile)
        {
            return new TouristXpProfileDto
            {
                Id = profile.Id,
                UserId = profile.UserId,
                CurrentXP = profile.CurrentXP,
                Level = profile.Level,
                XpProgressInCurrentLevel = profile.GetXPProgressInCurrentLevel(),
                XpNeededForNextLevel = profile.GetXPNeededForNextLevel(),
                XpRequiredForNextLevel = profile.GetXPRequiredForNextLevel(),
                CanCreateEncounters = profile.CanCreateEncounters(),
                LevelUpHistory = profile.GetLevelUpHistory().Select(h => new LevelUpRecordDto
                {
                    Level = h.Level,
                    Timestamp = h.Timestamp
                }).ToList()
            };
        }

        /// <summary>
        /// Calculate distance between two points using Haversine formula
        /// </summary>
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000; // Earth radius in meters
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c; // Distance in meters
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
