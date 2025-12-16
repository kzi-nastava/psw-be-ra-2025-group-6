using System;

namespace Explorer.Stakeholders.API.Dtos
{
    public class ClubPostDto
    {
        public long Id { get; set; }
        public long AuthorId { get; set; }
        public long ClubId { get; set; }
        public string Text { get; set; }
        public long? ResourceId { get; set; }
        public ResourceTypeDto? ResourceType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
