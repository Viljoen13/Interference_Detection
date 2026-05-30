using InterferenceDetection.Domain.Entities;

namespace InterferenceDetection.Application.Abstractions;

public interface ICoverageAnalyzer
{
    CoverageOverlap Analyze(Cell a, Cell b);
}

public sealed record CoverageOverlap(bool Overlaps, double DistanceMeters, double OverlapMeters);
