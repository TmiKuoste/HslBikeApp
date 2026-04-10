using System.Text.Json;
using HslBikeApp.Models;

namespace HslBikeApp.Tests.Models;

public class DestinationTableTests
{
    [Fact]
    public void ParseRows_ConvertsColumnarRowsToTypedDestinations()
    {
        var json = """
            {
              "fields": ["arrivalStationId", "tripCount", "averageDurationSeconds", "averageDistanceMetres"],
              "rows": [
                ["002", 150, 480, 1200],
                ["003", 80, 600, 1800]
              ]
            }
            """;
        var table = JsonSerializer.Deserialize<DestinationTable>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        var parsed = table.ParseRows();

        Assert.Equal(2, parsed.Count);
        // Should be sorted by trip count descending
        Assert.Equal("002", parsed[0].ArrivalStationId);
        Assert.Equal(150, parsed[0].TripCount);
        Assert.Equal(480, parsed[0].AverageDurationSeconds);
        Assert.Equal(1200, parsed[0].AverageDistanceMetres);
    }

    [Fact]
    public void ParseRows_SortsByTripCountDescending()
    {
        var json = """
            {
              "fields": ["arrivalStationId", "tripCount", "averageDurationSeconds", "averageDistanceMetres"],
              "rows": [
                ["A", 10, 300, 500],
                ["B", 50, 300, 500],
                ["C", 30, 300, 500]
              ]
            }
            """;
        var table = JsonSerializer.Deserialize<DestinationTable>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        var parsed = table.ParseRows();

        Assert.Equal("B", parsed[0].ArrivalStationId);
        Assert.Equal("C", parsed[1].ArrivalStationId);
        Assert.Equal("A", parsed[2].ArrivalStationId);
    }

    [Fact]
    public void ParseRows_WhenRowsAreEmpty_ReturnsEmptyList()
    {
        var table = new DestinationTable
        {
            Fields = ["arrivalStationId", "tripCount", "averageDurationSeconds", "averageDistanceMetres"],
            Rows = []
        };

        Assert.Empty(table.ParseRows());
    }

    [Fact]
    public void ParseRows_SkipsRowsWithFewerThan4Columns()
    {
        var table = new DestinationTable
        {
            Fields = ["arrivalStationId", "tripCount", "averageDurationSeconds", "averageDistanceMetres"],
            Rows = [["only-two", (object?)10]]
        };

        Assert.Empty(table.ParseRows());
    }
}
