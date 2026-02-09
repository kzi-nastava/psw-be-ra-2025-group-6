using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class NotificationService : INotificationService, IInternalNotificationService
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
            // Parse Type from string, default to General if not provided
            var type = NotificationType.General;
            if (!string.IsNullOrEmpty(notificationDto.Type) && System.Enum.TryParse<NotificationType>(notificationDto.Type, out var parsedType))
            {
                type = parsedType;
            }

            var notification = new Notification(
                notificationDto.RecipientId,
                notificationDto.SenderId,
                notificationDto.Content,
                notificationDto.ReferenceId,
                type,
                notificationDto.Title
            );
            var result = _notificationRepository.Create(notification);
            return MapDto(_mapper.Map<NotificationDto>(result));
        }

        // Internal API implementation
        public void CreateNotification(long recipientId, long senderId, string content, long? referenceId, string type)
        {
            var notificationType = NotificationType.General;
            if (!string.IsNullOrEmpty(type) && System.Enum.TryParse<NotificationType>(type, out var parsedType))
            {
                notificationType = parsedType;
            }

            var notification = new Notification(recipientId, senderId, content, referenceId ?? 0, notificationType, null);
            _notificationRepository.Create(notification);
        }

        public void CreateNotificationsForMultipleRecipients(List<long> recipientIds, long senderId, string content, long? referenceId, string type)
        {
            var notificationType = NotificationType.General;
            if (!string.IsNullOrEmpty(type) && System.Enum.TryParse<NotificationType>(type, out var parsedType))
            {
                notificationType = parsedType;
            }

            foreach (var recipientId in recipientIds)
            {
                var notification = new Notification(recipientId, senderId, content, referenceId ?? 0, notificationType, null);
                _notificationRepository.Create(notification);
            }
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

        public List<NotificationDto> GetUnreadByRecipientAndType(long recipientId, string type)
        {
            if (string.IsNullOrEmpty(type) || !System.Enum.TryParse<NotificationType>(type, out var notificationType))
            {
                return new List<NotificationDto>();
            }

            var notifications = _notificationRepository.GetUnreadByRecipientAndType(recipientId, notificationType);
            return _mapper.Map<List<NotificationDto>>(notifications);
        }

        public List<NotificationDto> GetByRecipientAndType(long recipientId, string type)
        {
            if (string.IsNullOrEmpty(type) || !System.Enum.TryParse<NotificationType>(type, out var notificationType))
            {
                return new List<NotificationDto>();
            }

            var notifications = _notificationRepository.GetByRecipientAndType(recipientId, notificationType);
            return _mapper.Map<List<NotificationDto>>(notifications);
        }

        public List<NotificationDto> GetByRecipient(long recipientId)
        {
            var notifications = _notificationRepository.GetByRecipient(recipientId);
            return _mapper.Map<List<NotificationDto>>(notifications);
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
