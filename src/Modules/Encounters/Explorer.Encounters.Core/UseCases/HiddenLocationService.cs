using AutoMapper;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;

namespace Explorer.Encounters.Core.UseCases
{
    public class HiddenLocationService : IHiddenLocationService
    {
        private readonly IHiddenLocationAttemptRepository _attemptRepository;
        private readonly IChallengeRepository _challengeRepository;
        private readonly ITouristEncounterService _encounterService;
        private readonly IMapper _mapper;
        private const double REQUIRED_RADIUS_METERS = 5.0;

        public HiddenLocationService(
            IHiddenLocationAttemptRepository attemptRepository,
            IChallengeRepository challengeRepository,
            ITouristEncounterService encounterService,
            IMapper mapper)
        {
            _attemptRepository = attemptRepository;
            _challengeRepository = challengeRepository;
            _encounterService = encounterService;
            _mapper = mapper;
        }

        public HiddenLocationAttemptDto StartAttempt(StartHiddenLocationDto dto, long userId)
        {
            var challenge = _challengeRepository.Get(dto.ChallengeId);
            if (challenge == null)
                throw new KeyNotFoundException($"Challenge {dto.ChallengeId} not found.");

            if (challenge.Type != ChallengeType.Location)
                throw new InvalidOperationException("This challenge is not a Hidden Location type.");

            var existingAttempt = _attemptRepository.GetActiveAttempt(userId, dto.ChallengeId);
            if (existingAttempt != null)
                throw new InvalidOperationException("You already have an active attempt for this challenge.");

            var distance = CalculateDistance(dto.UserLatitude, dto.UserLongitude, challenge.Latitude, challenge.Longitude);
            if (distance > REQUIRED_RADIUS_METERS)
                throw new InvalidOperationException($"You must be within {REQUIRED_RADIUS_METERS}m of the location to start.");

            var attempt = new HiddenLocationAttempt(userId, dto.ChallengeId);
            attempt = _attemptRepository.Create(attempt);

            return _mapper.Map<HiddenLocationAttemptDto>(attempt);
        }

        public HiddenLocationProgressDto UpdateProgress(UpdateHiddenLocationProgressDto dto, long userId)
        {
            var attempt = _attemptRepository.Get(dto.AttemptId);
            if (attempt.UserId != userId)
                throw new UnauthorizedAccessException("This attempt does not belong to you.");

            if (attempt.CompletedAt.HasValue)
                throw new InvalidOperationException("This attempt is already completed.");

            var challenge = _challengeRepository.Get(attempt.ChallengeId);
            var distance = CalculateDistance(dto.UserLatitude, dto.UserLongitude, challenge.Latitude, challenge.Longitude);
            var isInRadius = distance <= REQUIRED_RADIUS_METERS;

            attempt.UpdateProgress(isInRadius);
            _attemptRepository.Update(attempt);

            int? xpAwarded = null;
            bool leveledUp = false;
            int? newLevel = null;

            if (attempt.IsSuccessful && !attempt.CompletedAt.HasValue)
            {
                Console.WriteLine($"[HIDDEN LOCATION] Challenge {challenge.Id} completed by user {userId}");
                
                var completeRequest = new CompleteEncounterRequestDto
                {
                    ChallengeId = challenge.Id,
                    CurrentLatitude = dto.UserLatitude,
                    CurrentLongitude = dto.UserLongitude
                };

                Console.WriteLine($"[HIDDEN LOCATION] Calling CompleteEncounter service...");
                var completionResult = _encounterService.CompleteEncounter(userId, completeRequest);
                Console.WriteLine($"[HIDDEN LOCATION] CompleteEncounter result: Success={completionResult.Success}, Message={completionResult.Message}");
                
                if (completionResult.Success)
                {
                    xpAwarded = completionResult.XpAwarded;
                    leveledUp = completionResult.LeveledUp;
                    newLevel = completionResult.NewLevel;
                    
                    Console.WriteLine($"[HIDDEN LOCATION] XP awarded: {xpAwarded}, LeveledUp: {leveledUp}, NewLevel: {newLevel}");
                }
                else
                {
                    Console.WriteLine($"[HIDDEN LOCATION] WARNING: CompleteEncounter failed! Reason: {completionResult.Message}");
                }

                attempt.Complete();
                _attemptRepository.Update(attempt);
            }

            return new HiddenLocationProgressDto
            {
                AttemptId = attempt.Id,
                SecondsInRadius = attempt.SecondsInRadius,
                IsSuccessful = attempt.IsSuccessful,
                IsInRadius = isInRadius,
                DistanceToTarget = distance,
                XpAwarded = xpAwarded,
                LeveledUp = leveledUp,
                NewLevel = newLevel
            };
        }

        public HiddenLocationAttemptDto? GetActiveAttempt(long userId, long challengeId)
        {
            var attempt = _attemptRepository.GetActiveAttempt(userId, challengeId);
            return attempt == null ? null : _mapper.Map<HiddenLocationAttemptDto>(attempt);
        }

        public List<HiddenLocationAttemptDto> GetUserAttempts(long userId, long challengeId)
        {
            var attempts = _attemptRepository.GetUserAttempts(userId, challengeId);
            return _mapper.Map<List<HiddenLocationAttemptDto>>(attempts);
        }

        public ActivationCheckDto CheckActivation(long challengeId, double userLatitude, double userLongitude)
        {
            var challenge = _challengeRepository.Get(challengeId);
            if (challenge == null)
                throw new KeyNotFoundException($"Challenge {challengeId} not found.");

            if (challenge.Type != ChallengeType.Location)
                throw new InvalidOperationException("This challenge is not a Hidden Location type.");

            var distance = CalculateDistance(userLatitude, userLongitude, challenge.Latitude, challenge.Longitude);
            var canActivate = distance <= challenge.ActivationRadiusMeters;

            return new ActivationCheckDto
            {
                CanActivate = canActivate,
                DistanceToTarget = distance,
                ActivationRadius = challenge.ActivationRadiusMeters,
                Message = canActivate 
                    ? "You can activate this challenge!" 
                    : $"You need to be within {challenge.ActivationRadiusMeters}m to activate. Current distance: {distance:F1}m"
            };
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371e3;
            var phi1 = lat1 * Math.PI / 180;
            var phi2 = lat2 * Math.PI / 180;
            var deltaPhi = (lat2 - lat1) * Math.PI / 180;
            var deltaLambda = (lon2 - lon1) * Math.PI / 180;

            var a = Math.Sin(deltaPhi / 2) * Math.Sin(deltaPhi / 2) +
                    Math.Cos(phi1) * Math.Cos(phi2) *
                    Math.Sin(deltaLambda / 2) * Math.Sin(deltaLambda / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }
    }
}
