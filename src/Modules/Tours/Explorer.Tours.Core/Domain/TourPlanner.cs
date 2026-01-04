using Explorer.BuildingBlocks.Core.Domain;
using System;

namespace Explorer.Tours.Core.Domain
{
    public class TourPlanner : Entity
    {
        public long UserId { get; set; }

        public long TourId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        private TourPlanner() { }

        public TourPlanner(long userId, long tourId, DateTime startDate, DateTime endDate)
        {
            UserId = userId;
            TourId = tourId;
            Validate(startDate, endDate);
            StartDate = startDate;
            EndDate = endDate;
        }

        public void Update(DateTime? startDate, DateTime? endDate)
        {
            var newStart = startDate ?? StartDate;
            var newEnd = endDate ?? EndDate;
            Validate(newStart, newEnd);
            StartDate = newStart;
            EndDate = newEnd;
        }

        private static void Validate(DateTime startDate, DateTime endDate)
        {
            if (startDate <= DateTime.Now || endDate <= DateTime.Now || startDate > endDate)
            {
                throw new ArgumentException("Incorrect date range.");
            }
        }
    }
}
