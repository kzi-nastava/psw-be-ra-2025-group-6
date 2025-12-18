using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class TrackPoint : ValueObject
    {
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        private TrackPoint() { }

        public TrackPoint(double latitude, double longitude)
        {
            if (latitude < -90 || latitude > 90) throw new ArgumentException("Invalid Latitude");
            if (longitude < -180 || longitude > 180) throw new ArgumentException("Invalid Longitude");

            Latitude = latitude;
            Longitude = longitude;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Latitude;
            yield return Longitude;
        }
    }
}
