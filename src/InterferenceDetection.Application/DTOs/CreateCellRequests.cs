namespace InterferenceDetection.Application.DTOs;

public sealed record CreateLteCellRequest(
    string Name,
    double Latitude,
    double Longitude,
    double CoverageRadiusMeters,
    double TransmitPowerDbm,
    int Earfcn,
    int PhysicalCellId,
    int TrackingAreaCode);

public sealed record CreateFiveGCellRequest(
    string Name,
    double Latitude,
    double Longitude,
    double CoverageRadiusMeters,
    double TransmitPowerDbm,
    int Nrarfcn,
    int PhysicalCellId,
    long GnbId,
    int TrackingAreaCode);
