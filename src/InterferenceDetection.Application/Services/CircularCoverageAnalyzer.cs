using InterferenceDetection.Application.Abstractions;
using InterferenceDetection.Domain.Entities;

namespace InterferenceDetection.Application.Services;

public sealed class CircularCoverageAnalyzer(IDistanceCalculator distanceCalculator) : ICoverageAnalyzer
{
    public CoverageOverlap Analyze(Cell a, Cell b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);

        var distance = distanceCalculator.DistanceMeters(a.Location, b.Location);
        var combinedRadius = a.CoverageRadiusMeters + b.CoverageRadiusMeters;
        var overlap = Math.Max(0, combinedRadius - distance);
        return new CoverageOverlap(overlap > 0, distance, overlap);
    }
}
