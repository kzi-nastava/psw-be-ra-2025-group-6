using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.API.Controllers
{
    [Authorize]
    [Route("api/notifications")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ITourRecommendationService _recommendationService;
        private readonly ITouristPreferencesService _preferencesService;

        public NotificationController(
            INotificationService notificationService,
            ITourRecommendationService recommendationService,
            ITouristPreferencesService preferencesService)
        {
            _notificationService = notificationService;
            _recommendationService = recommendationService;
            _preferencesService = preferencesService;
        }

        /// <summary>
        /// Get all unread notifications for current user
        /// </summary>
        [HttpGet("unread")]
        public ActionResult<List<NotificationDto>> GetUnread()
        {
            TryCreateRecommendationsNotification();
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
        [HttpGet]
        public ActionResult<List<NotificationDto>> GetAll()
        {
            var result = _notificationService.GetByRecipient(User.PersonId());
            return Ok(result);
        }

        [HttpGet]
        public ActionResult<List<NotificationDto>> Get([FromQuery] int? limit = null)
        {
            TryCreateRecommendationsNotification();
            var result = _notificationService.GetByRecipient(User.PersonId(), limit);
            return Ok(result);
        }

        [HttpGet("me")]
        public ActionResult<List<NotificationDto>> GetMine([FromQuery] int? limit = null)
        {
            TryCreateRecommendationsNotification();
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

        [HttpPut("{notificationId:long}")]
        [HttpPatch("{notificationId:long}")]
        public ActionResult<NotificationDto> MarkAsReadLegacy(long notificationId)
        {
            var result = _notificationService.MarkAsRead(notificationId);
            return Ok(result);
        }

        private void TryCreateRecommendationsNotification()
        {
            if (!User.IsInRole("Tourist")) return;

            var touristId = User.PersonId();
            var preferences = _preferencesService.GetByTouristId(touristId);
            if (preferences == null) return;

            var summary = _recommendationService.GetRecommendationSummary(touristId, null);
            if (summary.NewMatchingCount <= 0) return;

            var newestPublishedAt = summary.NewestMatchingPublishedAt;
            if (newestPublishedAt == null) return;

            if (preferences.LastNotifiedAt.HasValue && preferences.LastNotifiedAt.Value >= newestPublishedAt.Value) return;

            var referenceId = summary.NewestMatchingTourId ?? 0;
            var senderId = summary.NewestMatchingTourAuthorId ?? 0;
            if (referenceId == 0 || senderId == 0) return;

            var content = $"Added {summary.NewMatchingCount} new tours for you. Open recommendations to view.";
            _notificationService.Create(new NotificationDto
            {
                RecipientId = touristId,
                SenderId = senderId,
                Title = "New tour recommendations",
                Content = content,
                ReferenceId = referenceId
            });

            _preferencesService.MarkRecommendationsNotified(touristId, newestPublishedAt.Value);
        }
    }
}
