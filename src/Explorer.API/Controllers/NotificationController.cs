using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Explorer.API.Controllers
{
    [Authorize(Policy = "registeredUserPolicy")]
    [Route("api/notifications")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("unread")]
        public ActionResult<List<NotificationDto>> GetUnread()
        {
            var result = _notificationService.GetUnreadByRecipient(User.PersonId());
            return Ok(result);
        }

        [HttpGet("me")]
        public ActionResult<List<NotificationDto>> GetMine([FromQuery] int? limit = null)
        {
            var result = _notificationService.GetByRecipient(User.PersonId(), limit);
            return Ok(result);
        }

        [HttpPut("{notificationId:long}/mark-as-read")]
        public ActionResult<NotificationDto> MarkAsRead(long notificationId)
        {
            var result = _notificationService.MarkAsRead(notificationId);
            return Ok(result);
        }

        [HttpPut("{notificationId:long}/read")]
        public ActionResult<NotificationDto> MarkAsReadNew(long notificationId)
        {
            var result = _notificationService.MarkAsRead(notificationId);
            return Ok(result);
        }
    }
}
