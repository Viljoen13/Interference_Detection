using InterferenceDetection.Domain.Enums;
using InterferenceDetection.Domain.ValueObjects;

namespace InterferenceDetection.Domain.Entities;

public abstract class Cell
{
    public Guid Id { get; protected set; }
    public string Name { get; protected set; } = default!;
    public GeoCoordinate Location { get; protected set; } = default!;
    public double CoverageRadiusMeters { get; protected set; }
    public double TransmitPowerDbm { get; protected set; }

    public abstract CellTechnology Technology { get; }

    protected Cell() { }

    protected Cell(
        Guid id,
        string name,
        GeoCoordinate location,
        double coverageRadiusMeters,
        double transmitPowerDbm)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Cell name is required.", nameof(name));
        if (coverageRadiusMeters <= 0)
            throw new ArgumentOutOfRangeException(nameof(coverageRadiusMeters), "Coverage radius must be positive.");

        Id = id;
        Name = name;
        Location = location ?? throw new ArgumentNullException(nameof(location));
        CoverageRadiusMeters = coverageRadiusMeters;
        TransmitPowerDbm = transmitPowerDbm;
    }

    public void Relocate(GeoCoordinate location)
        => Location = location ?? throw new ArgumentNullException(nameof(location));

    public void UpdateCoverageRadius(double radiusMeters)
    {
        if (radiusMeters <= 0)
            throw new ArgumentOutOfRangeException(nameof(radiusMeters), "Coverage radius must be positive.");
        CoverageRadiusMeters = radiusMeters;
    }
}
