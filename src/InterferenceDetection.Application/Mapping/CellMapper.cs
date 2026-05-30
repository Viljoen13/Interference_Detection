using InterferenceDetection.Application.DTOs;
using InterferenceDetection.Domain.Entities;

namespace InterferenceDetection.Application.Mapping;

public static class CellMapper
{
    public static CellDto ToDto(Cell cell) => cell switch
    {
        LteCell l => new LteCellDto(
            l.Id, l.Name, l.Location.Latitude, l.Location.Longitude,
            l.CoverageRadiusMeters, l.TransmitPowerDbm,
            l.Earfcn, l.PhysicalCellId, l.TrackingAreaCode),
        FiveGCell g => new FiveGCellDto(
            g.Id, g.Name, g.Location.Latitude, g.Location.Longitude,
            g.CoverageRadiusMeters, g.TransmitPowerDbm,
            g.Nrarfcn, g.PhysicalCellId, g.GnbId, g.TrackingAreaCode),
        _ => throw new NotSupportedException($"No DTO mapping registered for {cell.GetType().Name}.")
    };
}
