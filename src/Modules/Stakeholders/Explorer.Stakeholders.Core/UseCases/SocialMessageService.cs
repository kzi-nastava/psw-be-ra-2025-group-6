using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.AspNetCore.Http;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class SocialMessageService : ISocialMessageService
    {
        private readonly ISocialMessageRepository _socialMessageRepository;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SocialMessageService(ISocialMessageRepository socialMessageRepository,
                                    INotificationService notificationService,
                                    IMapper mapper,
                                    IHttpContextAccessor httpContextAccessor)
        {
            _socialMessageRepository = socialMessageRepository;
            _notificationService = notificationService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<SocialMessageDto> Create(SocialMessageDto messageDto)
        {
            var message = new SocialMessage(messageDto.SenderId, messageDto.ReceiverId, messageDto.Content);
            var result = _socialMessageRepository.Create(message);

            _notificationService.Create(new NotificationDto
            {
                RecipientId = messageDto.ReceiverId,
                SenderId = messageDto.SenderId,
                Content = $"New message from user {messageDto.SenderId}",
                ReferenceId = messageDto.SenderId,
            });

            return _mapper.Map<SocialMessageDto>(result);
        }

        public async Task<PagedResult<SocialMessageDto>> GetConversation(long userId1, long userId2, int page, int pageSize)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == "personId");
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
                throw new ForbiddenException("User ID not found in token.");

            if (userId != userId1 && userId != userId2)
                throw new ForbiddenException("You are not authorized to view this conversation.");

            var result = _socialMessageRepository.GetConversation(userId1, userId2, page, pageSize);
            return _mapper.Map<PagedResult<SocialMessageDto>>(result);
        }
    }
}
