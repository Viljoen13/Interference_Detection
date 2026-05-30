using InterferenceDetection.Domain.ValueObjects;

namespace InterferenceDetection.Application.Abstractions;

public interface IDistanceCalculator
{
    double DistanceMeters(GeoCoordinate a, GeoCoordinate b);
}
