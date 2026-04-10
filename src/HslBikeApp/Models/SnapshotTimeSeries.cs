using System.Text.Json.Serialization;

namespace HslBikeApp.Models;

/// <summary>
/// Columnar snapshot response from GET /api/snapshots.
/// Each row is [stationId, count0, count1, …, countN] aligned with <see cref="Timestamps"/>.
/// </summary>
public record SnapshotTimeSeries
{
    public int IntervalMinutes { get; init; }

    public IReadOnlyList<DateTime> Timestamps { get; init; } = [];

    [JsonPropertyName("rows")]
    public IReadOnlyList<IReadOnlyList<object?>> RawRows { get; init; } = [];

    /// <summary>
    /// Parses <see cref="RawRows"/> into typed per-station series.
    /// Call once after deserialisation.
    /// </summary>
    public IReadOnlyList<StationCountSeries> ParseRows()
    {
        var result = new List<StationCountSeries>(RawRows.Count);
        foreach (var row in RawRows)
        {
            if (row.Count == 0) continue;

            var stationId = row[0]?.ToString() ?? "";
            var counts = new int[row.Count - 1];
            for (var i = 1; i < row.Count; i++)
            {
                counts[i - 1] = row[i] switch
                {
                    int n => n,
                    long l => (int)l,
                    double d => (int)d,
                    System.Text.Json.JsonElement je => je.TryGetInt32(out var v) ? v : 0,
                    _ => 0
                };
            }

            result.Add(new StationCountSeries { StationId = stationId, Counts = counts });
        }

        return result;
    }

    /// <summary>
    /// Detects whether a gap exists between two consecutive timestamps.
    /// A gap is defined as a time difference exceeding <c>intervalMinutes × 1.5</c>.
    /// </summary>
    public bool IsGap(int index)
    {
        if (index < 1 || index >= Timestamps.Count) return false;
        var diff = (Timestamps[index] - Timestamps[index - 1]).TotalMinutes;
        return diff > IntervalMinutes * 1.5;
    }
}

/// <summary>
/// A single station's bike counts aligned with the parent <see cref="SnapshotTimeSeries.Timestamps"/>.
/// </summary>
public record StationCountSeries
{
    public required string StationId { get; init; }
    public required int[] Counts { get; init; }
}
