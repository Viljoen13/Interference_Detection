using InterferenceDetection.Domain.Enums;
using InterferenceDetection.Domain.ValueObjects;

namespace InterferenceDetection.Domain.Entities;

public sealed class LteCell : Cell
{
    public int Earfcn { get; private set; }
    public int PhysicalCellId { get; private set; }
    public int TrackingAreaCode { get; private set; }

    public override CellTechnology Technology => CellTechnology.Lte;

    private LteCell() { }

    public LteCell(
        Guid id,
        string name,
        GeoCoordinate location,
        double coverageRadiusMeters,
        double transmitPowerDbm,
        int earfcn,
        int physicalCellId,
        int trackingAreaCode)
        : base(id, name, location, coverageRadiusMeters, transmitPowerDbm)
    {
        if (physicalCellId is < 0 or > 503)
            throw new ArgumentOutOfRangeException(nameof(physicalCellId), "LTE PCI must be in [0, 503].");
        if (earfcn < 0)
            throw new ArgumentOutOfRangeException(nameof(earfcn), "EARFCN must be non-negative.");

        Earfcn = earfcn;
        PhysicalCellId = physicalCellId;
        TrackingAreaCode = trackingAreaCode;
    }
}
