using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public enum JournalStatus
    {
        Draft,
        Published
    }

    public class Journal : Entity
    {
        public long TouristId { get; init; }
        public string Name { get; init; }
        public string? Location { get; init; }
        public DateTime TravelDate { get; private set; }
        public JournalStatus Status { get; init; }
        public DateTime DateCreated { get; init; }
        public DateTime DateModified { get; private set; }

        private Journal() { }

        public Journal(long turistId, string name, string? location, DateTime travelDate)
        {
            if (turistId <=0)
                throw new ArgumentException("Turist ID cannot be empty.", nameof(turistId));

            if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
                throw new ArgumentException("Name is required and must be max 100 characters.", nameof(name));

            if (travelDate > DateTime.Now.Date) 
                throw new ArgumentException("Travel date cannot be in the future.", nameof(travelDate));


            TouristId = turistId;
            Name = name;
            Location = location;
            TravelDate = travelDate;
            Status = JournalStatus.Draft; 
            DateCreated = DateTime.UtcNow;
            DateModified = DateTime.UtcNow;
        }



    }
}
