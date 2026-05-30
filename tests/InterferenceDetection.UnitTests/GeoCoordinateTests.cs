using FluentAssertions;
using InterferenceDetection.Domain.ValueObjects;

namespace InterferenceDetection.UnitTests;

public class GeoCoordinateTests
{
    [Theory]
    [InlineData(-91, 0)]
    [InlineData(91, 0)]
    [InlineData(0, -181)]
    [InlineData(0, 181)]
    public void Out_Of_Range_Coordinates_Throw(double lat, double lon)
    {
        var act = () => new GeoCoordinate(lat, lon);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Records_With_Same_Values_Are_Equal()
    {
        new GeoCoordinate(-33.9249, 18.4241).Should().Be(new GeoCoordinate(-33.9249, 18.4241));
    }
}
