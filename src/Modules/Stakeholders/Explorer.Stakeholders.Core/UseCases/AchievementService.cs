using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class AchievementService : IAchievementService
    {
        private readonly IAchievementRepository _repository;
        private readonly IMapper _mapper;

        public AchievementService(IAchievementRepository repository,  IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public List<AchievementDto> GetAll()
        {
            return _mapper.Map<List<AchievementDto>>(_repository.GetAll());
        }

        public AchievementDto GetById(long id)
        {
            var achievement = _repository.GetById(id)
                ?? throw new KeyNotFoundException("Achievement not found.");

            return _mapper.Map<AchievementDto>(achievement);
        }
    }
}
