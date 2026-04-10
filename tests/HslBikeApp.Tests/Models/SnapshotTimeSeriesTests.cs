using System.Text.Json;
using HslBikeApp.Models;

namespace HslBikeApp.Tests.Models;

public class SnapshotTimeSeriesTests
{
    [Fact]
    public void ParseRows_ConvertsColumnarRowsToTypedSeries()
    {
        var json = """
            {
              "intervalMinutes": 15,
              "timestamps": ["2026-06-01T10:00:00Z", "2026-06-01T10:15:00Z"],
              "rows": [
                ["001", 5, 8],
                ["002", 3, 6]
              ]
            }
            """;
        var timeSeries = JsonSerializer.Deserialize<SnapshotTimeSeries>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        var parsed = timeSeries.ParseRows();

        Assert.Equal(2, parsed.Count);
        Assert.Equal("001", parsed[0].StationId);
        Assert.Equal([5, 8], parsed[0].Counts);
        Assert.Equal("002", parsed[1].StationId);
        Assert.Equal([3, 6], parsed[1].Counts);
    }

    [Fact]
    public void ParseRows_WhenRowsAreEmpty_ReturnsEmptyList()
    {
        var timeSeries = new SnapshotTimeSeries
        {
            IntervalMinutes = 15,
            Timestamps = [],
            RawRows = []
        };

        var parsed = timeSeries.ParseRows();

        Assert.Empty(parsed);
    }

    [Fact]
    public void ParseRows_HandlesJsonElementValues()
    {
        // Simulate what ReadFromJsonAsync produces: JsonElement values in the rows
        var json = """
            {
              "intervalMinutes": 15,
              "timestamps": ["2026-06-01T10:00:00Z"],
              "rows": [["station-A", 42]]
            }
            """;
        var timeSeries = JsonSerializer.Deserialize<SnapshotTimeSeries>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        var parsed = timeSeries.ParseRows();

        Assert.Single(parsed);
        Assert.Equal("station-A", parsed[0].StationId);
        Assert.Equal([42], parsed[0].Counts);
    }

    [Fact]
    public void IsGap_WhenIndexIsZero_ReturnsFalse()
    {
        var timeSeries = new SnapshotTimeSeries
        {
            IntervalMinutes = 15,
            Timestamps = [DateTime.UtcNow, DateTime.UtcNow.AddHours(2)]
        };

        Assert.False(timeSeries.IsGap(0));
    }

    [Fact]
    public void IsGap_WhenConsecutiveTimestamps_ReturnsFalse()
    {
        var baseTime = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc);
        var timeSeries = new SnapshotTimeSeries
        {
            IntervalMinutes = 15,
            Timestamps = [baseTime, baseTime.AddMinutes(15)]
        };

        Assert.False(timeSeries.IsGap(1));
    }

    [Fact]
    public void IsGap_WhenGapExceeds1Point5xInterval_ReturnsTrue()
    {
        var baseTime = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc);
        var timeSeries = new SnapshotTimeSeries
        {
            IntervalMinutes = 15,
            Timestamps = [baseTime, baseTime.AddMinutes(30)]
        };

        Assert.True(timeSeries.IsGap(1));
    }

    [Fact]
    public void IsGap_WhenOutOfBounds_ReturnsFalse()
    {
        var timeSeries = new SnapshotTimeSeries
        {
            IntervalMinutes = 15,
            Timestamps = [DateTime.UtcNow]
        };

        Assert.False(timeSeries.IsGap(5));
    }
}
