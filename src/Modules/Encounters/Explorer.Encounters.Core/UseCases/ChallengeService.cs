using AutoMapper;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Explorer.Encounters.API.Dtos;
using System.Collections.Generic;
using System;

namespace Explorer.Encounters.Core.UseCases
{
    public class ChallengeService : IChallengeService
    {
        private readonly IChallengeRepository _repository;
        private readonly ITouristXpProfileRepository _profileRepository;
        private readonly IMapper _mapper;

        public ChallengeService(IChallengeRepository repository, ITouristXpProfileRepository profileRepository, IMapper mapper)
        {
            _repository = repository;
            _profileRepository = profileRepository;
            _mapper = mapper;
        }

        public List<ChallengeDto> GetActive()
        {
            return _mapper.Map<List<ChallengeDto>>(_repository.GetAllActive());
        }

        public List<ChallengeDto> GetAll()
        {
            return _mapper.Map<List<ChallengeDto>>(_repository.GetAll());
        }

        public ChallengeDto Create(ChallengeDto dto)
        {
            var domain = _mapper.Map<Challenge>(dto);
            var created = _repository.Create(domain);
            return _mapper.Map<ChallengeDto>(created);
        }

        public ChallengeDto CreateByTourist(ChallengeDto dto, long touristId)
        {
            // Check if tourist is level 10 or higher
            var profile = _profileRepository.GetByUserId(touristId);
            if (profile == null || !profile.CanCreateEncounters())
            {
                throw new InvalidOperationException("You must be level 10 or higher to create encounters.");
            }

            // Tourist-created challenges are Misc type and start as Draft
            if (!Enum.TryParse<ChallengeType>(dto.Type, true, out var parsedType))
            {
                parsedType = ChallengeType.Misc;
            }

            var challenge = new Challenge(
                dto.Title,
                dto.Description,
                dto.Longitude,
                dto.Latitude,
                dto.XP,
                parsedType,
                touristId
            );

            var created = _repository.Create(challenge);
            return _mapper.Map<ChallengeDto>(created);
        }

        public List<ChallengeDto> GetPendingApproval()
        {
            return _mapper.Map<List<ChallengeDto>>(_repository.GetPendingApproval());
        }

        public ChallengeDto ApproveChallenge(long id)
        {
            var challenge = _repository.Get(id);
            if (challenge == null) throw new KeyNotFoundException("Challenge not found.");
            
            if (!challenge.IsCreatedByTourist)
                throw new InvalidOperationException("Only tourist-created challenges need approval.");

            challenge.Publish();
            var updated = _repository.Update(challenge);
            return _mapper.Map<ChallengeDto>(updated);
        }

        public ChallengeDto RejectChallenge(long id)
        {
            var challenge = _repository.Get(id);
            if (challenge == null) throw new KeyNotFoundException("Challenge not found.");
            
            if (!challenge.IsCreatedByTourist)
                throw new InvalidOperationException("Only tourist-created challenges can be rejected.");

            challenge.Archive();
            var updated = _repository.Update(challenge);
            return _mapper.Map<ChallengeDto>(updated);
        }

        public ChallengeDto Update(long id, ChallengeDto dto)
        {
            var existing = _repository.Get(id);
            if (existing == null) throw new KeyNotFoundException("Challenge not found.");

            if (!Enum.TryParse<ChallengeType>(dto.Type, true, out var parsedType))
                throw new ArgumentException("Invalid challenge type.");

            // Update data
            existing.Update(dto.Title, dto.Description, dto.Longitude, dto.Latitude, dto.XP, parsedType);

            // Allow status change if provided
            if (!string.IsNullOrWhiteSpace(dto.Status) && Enum.TryParse<ChallengeStatus>(dto.Status, true, out var parsedStatus))
            {
                existing.SetStatus(parsedStatus);
            }

            var updated = _repository.Update(existing);
            return _mapper.Map<ChallengeDto>(updated);
        }

        public void Delete(long id)
        {
            _repository.Delete(id);
        }

        public ChallengeDto Get(long id)
        {
            var c = _repository.Get(id);
            return _mapper.Map<ChallengeDto>(c);
        }
    }
}
