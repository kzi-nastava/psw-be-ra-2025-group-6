using Explorer.BuildingBlocks.Core.Domain;
using System;

namespace Explorer.Stakeholders.Core.Domain
{
    public class ClubPost : Entity
    {
        public long AuthorId { get; private set; }
        public long ClubId { get; private set; }
        public string Text { get; private set; }
        public long? ResourceId { get; private set; }
        public ResourceType? ResourceType { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public ClubPost(long authorId, long clubId, string text, long? resourceId, ResourceType? resourceType, DateTime createdAt, DateTime? updatedAt)
        {
            AuthorId = authorId;
            ClubId = clubId;
            Text = text;
            ResourceId = resourceId;
            ResourceType = resourceType;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            Validate();
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Text)) throw new ArgumentException("Invalid Text.");
            if (Text.Length > 280) throw new ArgumentException("Text exceeds the maximum length of 280 characters.");
            if (AuthorId == 0) throw new ArgumentException("Invalid AuthorId.");
            if (ClubId == 0) throw new ArgumentException("Invalid ClubId.");
        }

        public void Update(string text, long? resourceId, ResourceType? resourceType, DateTime? updatedAt)
        {
            Text = text;
            ResourceId = resourceId;
            ResourceType = resourceType;
            UpdatedAt = updatedAt;
            Validate();
        }
    }
}
