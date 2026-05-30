using FluentAssertions;
using InterferenceDetection.Application.Services;
using InterferenceDetection.Domain.Entities;
using InterferenceDetection.Domain.ValueObjects;

namespace InterferenceDetection.UnitTests;

public class FrequencyInterferenceDetectorTests
{
    private readonly FrequencyInterferenceDetector _sut =
        new(new CircularCoverageAnalyzer(new HaversineDistanceCalculator()));

    [Fact]
    public void Two_Lte_Cells_With_Same_Earfcn_And_Pci_Within_Coverage_Are_Pci_Collision()
    {
        var a = NewLte("a", -33.9249, 18.4241, coverage: 1500, earfcn: 1850, pci: 12);
        var b = NewLte("b", -33.9300, 18.4300, coverage: 1500, earfcn: 1850, pci: 12);

        var findings = _sut.Detect(new Cell[] { a, b });

        findings.Should().HaveCount(1);
        findings[0].Reason.Should().Be("LTE PCI collision on same EARFCN");
    }

    [Fact]
    public void Two_Lte_Cells_Same_Earfcn_Different_Pci_Are_Co_Channel_Overlap()
    {
        var a = NewLte("a", -33.9249, 18.4241, coverage: 1500, earfcn: 1850, pci: 12);
        var b = NewLte("b", -33.9255, 18.4250, coverage: 1500, earfcn: 1850, pci: 13);

        var findings = _sut.Detect(new Cell[] { a, b });

        findings.Should().HaveCount(1);
        findings[0].Reason.Should().Be("LTE co-channel coverage overlap");
    }

    [Fact]
    public void Different_Earfcn_Yields_No_Findings()
    {
        var a = NewLte("a", -33.9249, 18.4241, coverage: 1500, earfcn: 1850, pci: 12);
        var b = NewLte("b", -33.9255, 18.4250, coverage: 1500, earfcn: 6300, pci: 12);

        _sut.Detect(new Cell[] { a, b }).Should().BeEmpty();
    }

    [Fact]
    public void Cross_Technology_Pair_Is_Not_Reported()
    {
        var lte = NewLte("lte", -33.9249, 18.4241, coverage: 1500, earfcn: 1850, pci: 12);
        var fiveG = NewFiveG("5g", -33.9249, 18.4241, coverage: 800, nrarfcn: 632448, pci: 100);

        _sut.Detect(new Cell[] { lte, fiveG }).Should().BeEmpty();
    }

    [Fact]
    public void Two_5G_Cells_Same_Nrarfcn_And_Pci_Are_Pci_Collision()
    {
        var a = NewFiveG("a", -33.9249, 18.4241, coverage: 800, nrarfcn: 632448, pci: 100);
        var b = NewFiveG("b", -33.9255, 18.4250, coverage: 800, nrarfcn: 632448, pci: 100);

        var findings = _sut.Detect(new Cell[] { a, b });

        findings.Should().HaveCount(1);
        findings[0].Reason.Should().Be("5G NR PCI collision on same NR-ARFCN");
    }

    [Fact]
    public void No_Overlap_Means_No_Finding_Even_With_Pci_Collision()
    {
        // Same EARFCN/PCI but ~700km apart (Cape Town vs Johannesburg) — no coverage overlap.
        var a = NewLte("a", -33.9249, 18.4241, coverage: 1500, earfcn: 1850, pci: 12);
        var b = NewLte("b", -26.2041, 28.0473, coverage: 1500, earfcn: 1850, pci: 12);

        _sut.Detect(new Cell[] { a, b }).Should().BeEmpty();
    }

    private static LteCell NewLte(string name, double lat, double lon, double coverage, int earfcn, int pci) =>
        new(Guid.NewGuid(), name, new GeoCoordinate(lat, lon), coverage, 43, earfcn, pci, 12345);

    private static FiveGCell NewFiveG(string name, double lat, double lon, double coverage, int nrarfcn, int pci) =>
        new(Guid.NewGuid(), name, new GeoCoordinate(lat, lon), coverage, 38, nrarfcn, pci, 1, 22345);
}
