using InterferenceDetection.Domain.Enums;
using InterferenceDetection.Domain.ValueObjects;

namespace InterferenceDetection.Domain.Entities;

public sealed class FiveGCell : Cell
{
    public int Nrarfcn { get; private set; }
    public int PhysicalCellId { get; private set; }
    public long GnbId { get; private set; }
    public int TrackingAreaCode { get; private set; }

    public override CellTechnology Technology => CellTechnology.FiveG;

    private FiveGCell() { }

    public FiveGCell(
        Guid id,
        string name,
        GeoCoordinate location,
        double coverageRadiusMeters,
        double transmitPowerDbm,
        int nrarfcn,
        int physicalCellId,
        long gnbId,
        int trackingAreaCode)
        : base(id, name, location, coverageRadiusMeters, transmitPowerDbm)
    {
        if (physicalCellId is < 0 or > 1007)
            throw new ArgumentOutOfRangeException(nameof(physicalCellId), "5G NR PCI must be in [0, 1007].");
        if (nrarfcn < 0)
            throw new ArgumentOutOfRangeException(nameof(nrarfcn), "NR-ARFCN must be non-negative.");
        if (gnbId < 0)
            throw new ArgumentOutOfRangeException(nameof(gnbId), "gNB ID must be non-negative.");

        Nrarfcn = nrarfcn;
        PhysicalCellId = physicalCellId;
        GnbId = gnbId;
        TrackingAreaCode = trackingAreaCode;
    }
}
