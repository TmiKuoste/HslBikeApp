using HslBikeApp.Models;

namespace HslBikeApp.Tests.Models;

public class DestinationRowTests
{
    [Fact]
    public void AverageDurationFormatted_WhenMoreThanOneMinute_ReturnsMinutes()
    {
        var row = new DestinationRow
        {
            ArrivalStationId = "001",
            TripCount = 10,
            AverageDurationSeconds = 480,
            AverageDistanceMetres = 1200
        };

        Assert.Equal("8 min", row.AverageDurationFormatted);
    }

    [Fact]
    public void AverageDurationFormatted_WhenLessThanOneMinute_ReturnsLessThanOneMin()
    {
        var row = new DestinationRow
        {
            ArrivalStationId = "001",
            TripCount = 5,
            AverageDurationSeconds = 30,
            AverageDistanceMetres = 100
        };

        Assert.Equal("<1 min", row.AverageDurationFormatted);
    }

    [Fact]
    public void AverageDurationFormatted_WhenExactlyOneMinute_ReturnsOneMin()
    {
        var row = new DestinationRow
        {
            ArrivalStationId = "001",
            TripCount = 3,
            AverageDurationSeconds = 60,
            AverageDistanceMetres = 200
        };

        Assert.Equal("1 min", row.AverageDurationFormatted);
    }
}
