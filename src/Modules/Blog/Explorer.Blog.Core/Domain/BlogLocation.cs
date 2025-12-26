using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Blog.Core.Domain
{
    public class BlogLocation : AggregateRoot
    {
        public string City { get; private set; }
        public string Country { get; private set; }
        public string? Region { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        private BlogLocation() { }

        public BlogLocation(string city, string country, double latitude, double longitude, string? region = null)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City is required.");
            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentException("Country is required.");
            if (latitude < -90 || latitude > 90)
                throw new ArgumentException("Invalid latitude.");
            if (longitude < -180 || longitude > 180)
                throw new ArgumentException("Invalid longitude.");

            City = city;
            Country = country;
            Region = region;
            Latitude = latitude;
            Longitude = longitude;
        }

        public void UpdateLocation(string city, string country, double latitude, double longitude, string? region = null)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City is required.");
            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentException("Country is required.");
            if (latitude < -90 || latitude > 90)
                throw new ArgumentException("Invalid latitude.");
            if (longitude < -180 || longitude > 180)
                throw new ArgumentException("Invalid longitude.");

            City = city;
            Country = country;
            Region = region;
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
