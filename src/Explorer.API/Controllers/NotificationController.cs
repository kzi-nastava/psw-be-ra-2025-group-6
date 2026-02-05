using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// Get all unread notifications for current user
        /// </summary>
        [HttpGet("unread")]
        public ActionResult<List<NotificationDto>> GetUnread()
        {
            var result = _notificationService.GetUnreadByRecipient(User.PersonId());
            return Ok(result);
        }

        /// <summary>
        /// Get unread notifications by type (e.g., RankIncreased, MilestoneXP)
        /// </summary>
        [HttpGet("unread/by-type")]
        public ActionResult<List<NotificationDto>> GetUnreadByType([FromQuery] string type)
        {
            var result = _notificationService.GetUnreadByRecipientAndType(User.PersonId(), type);
            return Ok(result);
        }

        /// <summary>
        /// Get all notifications by type
        /// </summary>
        [HttpGet("by-type")]
        public ActionResult<List<NotificationDto>> GetByType([FromQuery] string type)
        {
            var result = _notificationService.GetByRecipientAndType(User.PersonId(), type);
            return Ok(result);
        }

        /// <summary>
        /// Get all leaderboard-related notifications
        /// </summary>
        [HttpGet("leaderboard")]
        public ActionResult<List<NotificationDto>> GetLeaderboardNotifications()
        {
            var leaderboardTypes = new[] 
            { 
                "RankIncreased", "RankDecreased", "EnteredTop10", "EnteredTop3", "BecameFirst",
                "MilestoneXP", "MilestoneChallenges", "MilestoneTours", 
                "ClubRankChanged", "NearRankingAlert"
            };

            var allNotifications = new List<NotificationDto>();
            foreach (var type in leaderboardTypes)
            {
                var notifications = _notificationService.GetByRecipientAndType(User.PersonId(), type);
                allNotifications.AddRange(notifications);
            }

            return Ok(allNotifications.OrderByDescending(n => n.Timestamp).ToList());
        }

        /// <summary>
        /// Mark notification as read
        /// </summary>
        [HttpPut("{notificationId:long}/mark-as-read")]
        public ActionResult<NotificationDto> MarkAsRead(long notificationId)
        {
            var result = _notificationService.MarkAsRead(notificationId);
            return Ok(result);
        }
    }
}
