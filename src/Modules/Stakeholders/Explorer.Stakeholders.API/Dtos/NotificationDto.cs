using System;

namespace Explorer.Stakeholders.API.Dtos
{
    public class NotificationDto
    {
        public long Id { get; set; }
        public long RecipientId { get; set; }
        public long SenderId { get; set; }
        public string Content { get; set; }
        public string Status { get; set; } // Map enum to string
        public DateTime Timestamp { get; set; }
        public long ReferenceId { get; set; }
    }
}
