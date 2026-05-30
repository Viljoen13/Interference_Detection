using InterferenceDetection.Application.Abstractions;
using InterferenceDetection.Domain.ValueObjects;

namespace InterferenceDetection.Application.Services;

public sealed class HaversineDistanceCalculator : IDistanceCalculator
{
    private const double EarthRadiusMeters = 6_371_000.0;

    public double DistanceMeters(GeoCoordinate a, GeoCoordinate b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);

        var lat1 = ToRadians(a.Latitude);
        var lat2 = ToRadians(b.Latitude);
        var dLat = ToRadians(b.Latitude - a.Latitude);
        var dLon = ToRadians(b.Longitude - a.Longitude);

        var h = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
              + Math.Cos(lat1) * Math.Cos(lat2)
              * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(h), Math.Sqrt(1 - h));
        return EarthRadiusMeters * c;
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180.0;
}
