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
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public TourProblemMessageService(ITourProblemMessageRepository tourProblemMessageRepository, ITourProblemRepository tourProblemRepository, ITourService tourService, INotificationService notificationService, IMapper mapper)
        {
            _tourProblemMessageRepository = tourProblemMessageRepository;
            _tourProblemRepository = tourProblemRepository;
            _tourService = tourService;
            _notificationService = notificationService;
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

            var message = new TourProblemMessage(messageDto.TourProblemId, messageDto.SenderId, messageDto.Content);
            var result = _tourProblemMessageRepository.Create(message);

            // Create notification for the other party
            long recipientId;
            string notificationContent;

            if (messageDto.SenderId == problem.TouristId)
            {
                recipientId = tour.AuthorId;
                notificationContent = $"Tourist {problem.TouristId} sent a message regarding problem on tour '{tour.Name}'.";
            }
            else // Sender is author
            {
                recipientId = problem.TouristId;
                notificationContent = $"Author {tour.AuthorId} sent a message regarding your problem on tour '{tour.Name}'.";
            }

            _notificationService.Create(new NotificationDto
            {
                RecipientId = recipientId,
                SenderId = messageDto.SenderId,
                Content = notificationContent,
                ReferenceId = messageDto.TourProblemId // Link notification to the tour problem
            });

            return _mapper.Map<TourProblemMessageDto>(result);
        }

        public Task<PagedResult<TourProblemMessageDto>> GetForProblem(long problemId, int page, int pageSize)
        {
            var result = _tourProblemMessageRepository.GetForProblem(problemId, page, pageSize);
            return Task.FromResult(_mapper.Map<PagedResult<TourProblemMessageDto>>(result));
        }
    }
}