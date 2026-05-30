using FluentAssertions;
using InterferenceDetection.Application.Services;
using InterferenceDetection.Domain.ValueObjects;

namespace InterferenceDetection.UnitTests;

public class HaversineDistanceCalculatorTests
{
    private readonly HaversineDistanceCalculator _sut = new();

    [Fact]
    public void Distance_Between_Same_Point_Is_Zero()
    {
        var p = new GeoCoordinate(-33.9249, 18.4241);
        _sut.DistanceMeters(p, p).Should().BeApproximately(0, 0.001);
    }

    [Fact]
    public void Distance_London_To_Paris_Is_Approximately_344km()
    {
        var london = new GeoCoordinate(51.5074, -0.1278);
        var paris = new GeoCoordinate(48.8566, 2.3522);

        var meters = _sut.DistanceMeters(london, paris);

        meters.Should().BeApproximately(343_500, 2_000);
    }

    [Fact]
    public void Distance_Is_Symmetric()
    {
        var a = new GeoCoordinate(-33.9249, 18.4241);
        var b = new GeoCoordinate(-26.2041, 28.0473);

        _sut.DistanceMeters(a, b).Should().BeApproximately(_sut.DistanceMeters(b, a), 0.001);
    }
}
