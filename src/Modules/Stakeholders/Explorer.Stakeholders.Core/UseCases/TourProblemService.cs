using AutoMapper;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class TourProblemService : ITourProblemService
    {
        private readonly ITourProblemRepository _repository;
        private readonly IMapper _mapper;

        public TourProblemService(ITourProblemRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<TourProblemDto>> GetAll()
        {
            var problems = await _repository.GetAll();
            return problems.Select(_mapper.Map<TourProblemDto>).ToList();
        }

        public async Task<TourProblemDto> Create(TourProblemDto problemDto)
        {
            var problem = new TourProblem(
                problemDto.TourId,
                problemDto.TouristId,
                (ProblemCategory)problemDto.Category,
                (ProblemPriority)problemDto.Priority,
                problemDto.Description
            );

            var result = await _repository.Create(problem);
            return _mapper.Map<TourProblemDto>(result);
        }

        public async Task<List<TourProblemDto>> GetByTourist(long touristId)
        {
            var problems = await _repository.GetByTourist(touristId);
            return problems.Select(_mapper.Map<TourProblemDto>).ToList();
        }

        public async Task<TourProblemDto> Update(TourProblemDto problemDto)
        {
            var existingProblem = await _repository.GetById(problemDto.Id);

            if (existingProblem == null)
                throw new KeyNotFoundException($"Problem with ID {problemDto.Id} not found.");

            existingProblem.Update(
                (ProblemCategory)problemDto.Category,
                (ProblemPriority)problemDto.Priority,
                problemDto.Description
            );

            var result = await _repository.Update(existingProblem);
            return _mapper.Map<TourProblemDto>(result);
        }

        public async Task<TourProblemDto> SetDeadline(long id, DateTime deadlineUtc)
        {
            var problem = await _repository.GetById(id);

            if (problem == null)
                throw new KeyNotFoundException($"Problem with ID {id} not found.");

            problem.SetDeadline(deadlineUtc);

            var result = await _repository.Update(problem);
            return _mapper.Map<TourProblemDto>(result);
        }

        public async Task Delete(long id, long touristId)
        {
            var problem = await _repository.GetById(id);

            if (problem == null)
                throw new KeyNotFoundException($"Problem with ID {id} not found.");

            if (problem.TouristId != touristId)
                throw new UnauthorizedAccessException("You can only delete your own problems.");

            await _repository.Delete(id);
        }
    }
}
