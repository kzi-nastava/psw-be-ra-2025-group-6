using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain
{
    public class ReviewApp : Entity
    {
        public long UserId { get; init; }
        public int Rating { get; private set; }
        public string? Comment { get; private set; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; private set; }

        public ReviewApp(long userId, int rating, string? comment)
        {
            UserId = userId;
            Rating = rating;
            Comment = comment;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = null;
            Validate();
        }

        public void Update(int rating, string? comment)
        {
            Rating = rating;
            Comment = comment;
            UpdatedAt = DateTime.UtcNow;
            Validate();
        }
        private ReviewApp() { }
        private void Validate()
        {
            if (Rating < 1 || Rating > 5) throw new ArgumentException("Rating must be between 1 and 5.");
        }
    }
}
