using InterferenceDetection.Domain.Entities;

namespace InterferenceDetection.Application.Abstractions;

public interface IInterferenceDetector
{
    IReadOnlyList<InterferenceFinding> Detect(IReadOnlyList<Cell> cells);
}

public sealed record InterferenceFinding(
    Guid CellAId,
    Guid CellBId,
    string Reason,
    double DistanceMeters,
    double OverlapMeters);
