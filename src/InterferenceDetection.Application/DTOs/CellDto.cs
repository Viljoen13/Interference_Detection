using System.Text.Json.Serialization;

namespace InterferenceDetection.Application.DTOs;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "technology")]
[JsonDerivedType(typeof(LteCellDto), typeDiscriminator: "LTE")]
[JsonDerivedType(typeof(FiveGCellDto), typeDiscriminator: "5G")]
public abstract record CellDto(
    Guid Id,
    string Name,
    double Latitude,
    double Longitude,
    double CoverageRadiusMeters,
    double TransmitPowerDbm);

public sealed record LteCellDto(
    Guid Id,
    string Name,
    double Latitude,
    double Longitude,
    double CoverageRadiusMeters,
    double TransmitPowerDbm,
    int Earfcn,
    int PhysicalCellId,
    int TrackingAreaCode)
    : CellDto(Id, Name, Latitude, Longitude, CoverageRadiusMeters, TransmitPowerDbm);

public sealed record FiveGCellDto(
    Guid Id,
    string Name,
    double Latitude,
    double Longitude,
    double CoverageRadiusMeters,
    double TransmitPowerDbm,
    int Nrarfcn,
    int PhysicalCellId,
    long GnbId,
    int TrackingAreaCode)
    : CellDto(Id, Name, Latitude, Longitude, CoverageRadiusMeters, TransmitPowerDbm);
