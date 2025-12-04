using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.AspNetCore.Http;
using Explorer.BuildingBlocks.Core.Exceptions;
using System.Linq;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class TourProblemMessageService : ITourProblemMessageService
    {
        private readonly ITourProblemMessageRepository _tourProblemMessageRepository;
        private readonly ITourProblemRepository _tourProblemRepository;
        private readonly ITourInfoGateway _tourInfoGateway;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TourProblemMessageService(ITourProblemMessageRepository tourProblemMessageRepository, ITourProblemRepository tourProblemRepository, ITourInfoGateway tourInfoGateway, INotificationService notificationService, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _tourProblemMessageRepository = tourProblemMessageRepository;
            _tourProblemRepository = tourProblemRepository;
            _tourInfoGateway = tourInfoGateway;
            _notificationService = notificationService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TourProblemMessageDto> Create(TourProblemMessageDto messageDto)
        {
            var problem = await _tourProblemRepository.GetById(messageDto.TourProblemId);
            if (problem == null) throw new NotFoundException("Tour problem not found.");

            var tour = await _tourInfoGateway.GetById(problem.TourId);
            if (tour == null) throw new NotFoundException("Tour not found.");

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

        public async Task<PagedResult<TourProblemMessageDto>> GetForProblem(long problemId, int page, int pageSize)
        {
            var problem = await _tourProblemRepository.GetById(problemId);
            if (problem == null) throw new NotFoundException("Tour problem not found.");

            var tour = await _tourInfoGateway.GetById(problem.TourId);
            if (tour == null) throw new NotFoundException("Tour not found.");

            var userIdClaim = _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == "personId");
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            {
                throw new ForbiddenException("User ID not found in token.");
            }

            if (userId != problem.TouristId && userId != tour.AuthorId)
            {
                throw new ForbiddenException("You are not authorized to view this conversation.");
            }

            var result = _tourProblemMessageRepository.GetForProblem(problemId, page, pageSize);
            return _mapper.Map<PagedResult<TourProblemMessageDto>>(result);
        }
    }
}
