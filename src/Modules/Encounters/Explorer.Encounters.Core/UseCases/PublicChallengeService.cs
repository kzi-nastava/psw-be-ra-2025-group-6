using AutoMapper;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using System.Collections.Generic;
using Explorer.Encounters.API.Public;

namespace Explorer.Encounters.Core.UseCases
{
    public class PublicChallengeService : IChallengePublicService
    {
        private readonly IChallengeRepository _repository;
        private readonly IMapper _mapper;

        public PublicChallengeService(IChallengeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public List<ChallengeDto> GetActive()
        {
            return _mapper.Map<List<ChallengeDto>>(_repository.GetAllActive());
        }

        public ChallengeDto Get(long id)
        {
            var c = _repository.Get(id);
            return _mapper.Map<ChallengeDto>(c);
        }
    }
}
