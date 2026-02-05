using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public NotificationService(INotificationRepository notificationRepository, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        public NotificationDto Create(NotificationDto notificationDto)
        {
            var notification = new Notification(
                notificationDto.RecipientId,
                notificationDto.SenderId,
                notificationDto.Content,
                notificationDto.ReferenceId,
                notificationDto.Title
            );
            var result = _notificationRepository.Create(notification);
            return MapDto(_mapper.Map<NotificationDto>(result));
        }

        public List<NotificationDto> GetUnreadByRecipient(long recipientId)
        {
            var notifications = _notificationRepository.GetUnreadByRecipient(recipientId);
            return _mapper.Map<List<NotificationDto>>(notifications).Select(MapDto).ToList();
        }

        public List<NotificationDto> GetByRecipient(long recipientId, int? limit = null)
        {
            var notifications = _notificationRepository.GetByRecipient(recipientId, limit);
            return _mapper.Map<List<NotificationDto>>(notifications).Select(MapDto).ToList();
        }

        public NotificationDto MarkAsRead(long notificationId)
        {
            var notification = _notificationRepository.Get(notificationId);
            notification.MarkAsRead();
            var result = _notificationRepository.Update(notification);
            return MapDto(_mapper.Map<NotificationDto>(result));
        }

        private static NotificationDto MapDto(NotificationDto dto)
        {
            dto.Message = dto.Content;
            dto.IsRead = string.Equals(dto.Status, NotificationStatus.Read.ToString(), System.StringComparison.OrdinalIgnoreCase);
            dto.CreatedAt = dto.Timestamp;
            return dto;
        }
    }
}
