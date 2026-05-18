using HslBikeApp.Models;

namespace HslBikeApp.Tests.Models;

public class OpenDataTimeSeriesTests
{
    [Fact]
    public void LatestAvailable_WhenAllValuesAvailable_ReturnsLastEntry()
    {
        var series = CreateSeries(
            timestamps:
            [
                new DateTimeOffset(2026, 5, 18, 10, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2026, 5, 18, 10, 15, 0, TimeSpan.Zero)
            ],
            values: [180, 192]);

        var latest = series.LatestAvailable();

        Assert.NotNull(latest);
        Assert.Equal(new DateTimeOffset(2026, 5, 18, 10, 15, 0, TimeSpan.Zero), latest.Value.Timestamp);
        Assert.Equal(192, latest.Value.Value);
    }

    [Fact]
    public void LatestAvailable_WhenLatestValueIsSentinel_SkipsToPreviousAvailable()
    {
        var series = CreateSeries(
            timestamps:
            [
                new DateTimeOffset(2026, 5, 18, 10, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2026, 5, 18, 10, 15, 0, TimeSpan.Zero),
                new DateTimeOffset(2026, 5, 18, 10, 30, 0, TimeSpan.Zero)
            ],
            values: [180, 192, OpenDataTimeSeries.UnavailableSentinel]);

        var latest = series.LatestAvailable();

        Assert.NotNull(latest);
        Assert.Equal(new DateTimeOffset(2026, 5, 18, 10, 15, 0, TimeSpan.Zero), latest.Value.Timestamp);
        Assert.Equal(192, latest.Value.Value);
    }

    [Fact]
    public void LatestAvailable_WhenAllValuesAreSentinel_ReturnsNull()
    {
        var series = CreateSeries(
            timestamps:
            [
                new DateTimeOffset(2026, 5, 18, 10, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2026, 5, 18, 10, 15, 0, TimeSpan.Zero)
            ],
            values: [OpenDataTimeSeries.UnavailableSentinel, OpenDataTimeSeries.UnavailableSentinel]);

        Assert.Null(series.LatestAvailable());
    }

    [Fact]
    public void LatestAvailable_WhenSeriesIsEmpty_ReturnsNull()
    {
        var series = CreateSeries(timestamps: [], values: []);

        Assert.Null(series.LatestAvailable());
    }

    private static OpenDataTimeSeries CreateSeries(IReadOnlyList<DateTimeOffset> timestamps, IReadOnlyList<double> values) =>
        new()
        {
            SourceId = "test",
            DisplayName = "Test",
            Lat = 60.0,
            Lon = 25.0,
            AttributionUrl = "https://example.com",
            Timestamps = timestamps,
            Values = values
        };
}
