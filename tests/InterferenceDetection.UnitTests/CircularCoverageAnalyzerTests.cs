using FluentAssertions;
using InterferenceDetection.Application.Abstractions;
using InterferenceDetection.Application.Services;
using InterferenceDetection.Domain.Entities;
using InterferenceDetection.Domain.ValueObjects;
using NSubstitute;

namespace InterferenceDetection.UnitTests;

public class CircularCoverageAnalyzerTests
{
    private readonly IDistanceCalculator _distanceCalculator = Substitute.For<IDistanceCalculator>();

    [Fact]
    public void Cells_Closer_Than_Combined_Radii_Overlap()
    {
        _distanceCalculator.DistanceMeters(Arg.Any<GeoCoordinate>(), Arg.Any<GeoCoordinate>()).Returns(1000);
        var sut = new CircularCoverageAnalyzer(_distanceCalculator);

        var a = NewCell(coverage: 700);
        var b = NewCell(coverage: 700);

        var result = sut.Analyze(a, b);

        result.Overlaps.Should().BeTrue();
        result.DistanceMeters.Should().Be(1000);
        result.OverlapMeters.Should().Be(400);
    }

    [Fact]
    public void Cells_Farther_Than_Combined_Radii_Do_Not_Overlap()
    {
        _distanceCalculator.DistanceMeters(Arg.Any<GeoCoordinate>(), Arg.Any<GeoCoordinate>()).Returns(5000);
        var sut = new CircularCoverageAnalyzer(_distanceCalculator);

        var a = NewCell(coverage: 700);
        var b = NewCell(coverage: 700);

        var result = sut.Analyze(a, b);

        result.Overlaps.Should().BeFalse();
        result.OverlapMeters.Should().Be(0);
    }

    private static LteCell NewCell(double coverage) => new(
        Guid.NewGuid(),
        "test",
        new GeoCoordinate(0, 0),
        coverageRadiusMeters: coverage,
        transmitPowerDbm: 40,
        earfcn: 1850,
        physicalCellId: 1,
        trackingAreaCode: 1);
}
