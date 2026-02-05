using AutoMapper;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using System.Collections.Generic;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using System;
using System.Linq;

namespace Explorer.Encounters.Core.UseCases
{
    public class PublicChallengeService : IChallengePublicService
    {
        private readonly IChallengeRepository _repository;
        private readonly ISocialEncounterRepository _socialEncounterRepository;
        private readonly ITourStatusGateway _tourStatusGateway;
        private readonly IMapper _mapper;

        public PublicChallengeService(
            IChallengeRepository repository, 
            ISocialEncounterRepository socialEncounterRepository,
            ITourStatusGateway tourStatusGateway,
            IMapper mapper)
        {
            _repository = repository;
            _socialEncounterRepository = socialEncounterRepository;
            _tourStatusGateway = tourStatusGateway;
            _mapper = mapper;
        }

        public List<ChallengeDto> GetActive()
        {
            var allActiveChallenges = _repository.GetAllActive();
            
            // Filter out KeyPoint challenges from unpublished tours
            var filteredChallenges = new List<Challenge>();
            
            foreach (var challenge in allActiveChallenges)
            {
                // Non-KeyPoint challenges are always visible
                if (!challenge.KeyPointId.HasValue)
                {
                    filteredChallenges.Add(challenge);
                    continue;
                }

                // Check if tour is published (CONFIRMED status)
                var tourStatus = _tourStatusGateway.GetTourStatusByKeyPointId(challenge.KeyPointId.Value).Result;
                
                // Only include KeyPoint challenges if tour is published
                if (tourStatus == "CONFIRMED")
                {
                    filteredChallenges.Add(challenge);
                }
            }

            return _mapper.Map<List<ChallengeDto>>(filteredChallenges);
        }

        public ChallengeDto Get(long id)
        {
            var c = _repository.Get(id);
            return _mapper.Map<ChallengeDto>(c);
        }

        public ChallengeDto CreateForKeyPoint(ChallengeDto dto, long keyPointId, double longitude, double latitude, long authorId)
        {
            if (!Enum.TryParse<ChallengeType>(dto.Type, true, out var parsedType))
            {
                throw new ArgumentException("Invalid challenge type.");
            }

            var challenge = new Challenge(
                dto.Title,
                dto.Description,
                dto.XP,
                parsedType,
                keyPointId,
                authorId,
                dto.IsRequiredForSecret,
                dto.ImagePath,
                dto.ActivationRadiusMeters > 0 ? dto.ActivationRadiusMeters : 50
            );

            challenge.SetLocationFromKeyPoint(longitude, latitude);

            var created = _repository.Create(challenge);

            // If Social type, automatically create SocialEncounter
            if (parsedType == ChallengeType.Social)
            {
                var requiredPeople = dto.RequiredPeople ?? 3;
                var socialRadius = dto.SocialRadiusMeters ?? 100.0;

                var socialEncounter = new SocialEncounter(created.Id, requiredPeople, socialRadius);
                _socialEncounterRepository.Create(socialEncounter);
            }

            return _mapper.Map<ChallengeDto>(created);
        }

        public List<ChallengeDto> GetByKeyPointId(long keyPointId)
        {
            var challenges = _repository.GetByKeyPointId(keyPointId);
            return _mapper.Map<List<ChallengeDto>>(challenges);
        }
    }
}
