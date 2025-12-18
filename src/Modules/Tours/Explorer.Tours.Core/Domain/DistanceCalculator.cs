namespace Explorer.Tours.Core.Domain
{
    public static class DistanceCalculator
    {
        private const double EarthRadiusKm = 6371.0;

        /// <summary>
        /// Calculates the distance between two GPS coordinates using the Haversine formula.
        /// </summary>
        /// <param name="lat1">Latitude of the first point</param>
        /// <param name="lon1">Longitude of the first point</param>
        /// <param name="lat2">Latitude of the second point</param>
        /// <param name="lon2">Longitude of the second point</param>
        /// <returns>Distance in meters</returns>
        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            var distanceInKm = EarthRadiusKm * c;
            return distanceInKm * 1000; // Convert to meters
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
}
