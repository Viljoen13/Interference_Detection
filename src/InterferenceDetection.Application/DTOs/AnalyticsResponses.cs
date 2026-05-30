namespace InterferenceDetection.Application.DTOs;

public sealed record DistanceResponse(Guid CellAId, Guid CellBId, double DistanceMeters);

public sealed record CoverageOverlapResponse(
    Guid CellAId,
    Guid CellBId,
    double DistanceMeters,
    double OverlapMeters,
    bool Overlaps);

public sealed record InterferenceFindingResponse(
    Guid CellAId,
    Guid CellBId,
    string Reason,
    double DistanceMeters,
    double OverlapMeters);
