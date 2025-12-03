using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.API.Dtos;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Exceptions;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class TourProblemMessageService : ITourProblemMessageService
    {
        private readonly ITourProblemMessageRepository _tourProblemMessageRepository;
        private readonly ITourProblemRepository _tourProblemRepository;
        private readonly ITourService _tourService;
        private readonly IMapper _mapper;

        public TourProblemMessageService(ITourProblemMessageRepository tourProblemMessageRepository, ITourProblemRepository tourProblemRepository, ITourService tourService, IMapper mapper)
        {
            _tourProblemMessageRepository = tourProblemMessageRepository;
            _tourProblemRepository = tourProblemRepository;
            _tourService = tourService;
            _mapper = mapper;
        }

        public async Task<TourProblemMessageDto> Create(TourProblemMessageDto messageDto)
        {
            var problem = await _tourProblemRepository.GetById(messageDto.TourProblemId);
            if (problem == null) throw new NotFoundException("Tour problem not found.");

            var tour = _tourService.Get(problem.TourId);

            if (messageDto.SenderId != problem.TouristId && messageDto.SenderId != tour.AuthorId)
            {
                throw new System.ArgumentException("User is not authorized to comment on this problem.");
            }

            // Manually create the domain entity to ensure constructor logic is run
            var message = new TourProblemMessage(messageDto.TourProblemId, messageDto.SenderId, messageDto.Content);

            var result = _tourProblemMessageRepository.Create(message);
            return _mapper.Map<TourProblemMessageDto>(result);
        }

        public Task<PagedResult<TourProblemMessageDto>> GetForProblem(long problemId, int page, int pageSize)
        {
            var result = _tourProblemMessageRepository.GetForProblem(problemId, page, pageSize);
            return Task.FromResult(_mapper.Map<PagedResult<TourProblemMessageDto>>(result));
        }
    }
}