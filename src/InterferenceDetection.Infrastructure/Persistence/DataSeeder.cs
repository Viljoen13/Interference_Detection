using InterferenceDetection.Domain.Entities;
using InterferenceDetection.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace InterferenceDetection.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        if (await db.Cells.AnyAsync(ct)) return;

        // Cape Town CBD-ish coordinates. Two cells are intentionally placed close together
        // on the same EARFCN/PCI to demonstrate the interference detector.
        var cells = new Cell[]
        {
            new LteCell(
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                "CT_LTE_001",
                new GeoCoordinate(-33.9249, 18.4241),
                coverageRadiusMeters: 1500, transmitPowerDbm: 43,
                earfcn: 1850, physicalCellId: 12, trackingAreaCode: 12345),

            new LteCell(
                Guid.Parse("22222222-2222-2222-2222-222222222222"),
                "CT_LTE_002",
                new GeoCoordinate(-33.9300, 18.4300),
                coverageRadiusMeters: 1200, transmitPowerDbm: 42,
                earfcn: 1850, physicalCellId: 12, trackingAreaCode: 12345),

            new LteCell(
                Guid.Parse("33333333-3333-3333-3333-333333333333"),
                "CT_LTE_003",
                new GeoCoordinate(-33.9100, 18.4500),
                coverageRadiusMeters: 1300, transmitPowerDbm: 43,
                earfcn: 6300, physicalCellId: 250, trackingAreaCode: 12346),

            new FiveGCell(
                Guid.Parse("44444444-4444-4444-4444-444444444444"),
                "CT_5G_001",
                new GeoCoordinate(-33.9249, 18.4241),
                coverageRadiusMeters: 800, transmitPowerDbm: 38,
                nrarfcn: 632448, physicalCellId: 100, gnbId: 1, trackingAreaCode: 22345),

            new FiveGCell(
                Guid.Parse("55555555-5555-5555-5555-555555555555"),
                "CT_5G_002",
                new GeoCoordinate(-33.9255, 18.4250),
                coverageRadiusMeters: 700, transmitPowerDbm: 38,
                nrarfcn: 632448, physicalCellId: 100, gnbId: 2, trackingAreaCode: 22345),

            new FiveGCell(
                Guid.Parse("66666666-6666-6666-6666-666666666666"),
                "CT_5G_003",
                new GeoCoordinate(-33.9180, 18.4400),
                coverageRadiusMeters: 900, transmitPowerDbm: 39,
                nrarfcn: 643680, physicalCellId: 200, gnbId: 3, trackingAreaCode: 22346),
        };

        await db.Cells.AddRangeAsync(cells, ct);
        await db.SaveChangesAsync(ct);
    }
}
