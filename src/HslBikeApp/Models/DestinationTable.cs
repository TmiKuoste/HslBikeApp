using System.Text.Json;

namespace HslBikeApp.Models;

/// <summary>
/// Raw columnar destinations table from the statistics endpoint.
/// </summary>
public record DestinationTable
{
    public IReadOnlyList<string> Fields { get; init; } = [];
    public IReadOnlyList<IReadOnlyList<object?>> Rows { get; init; } = [];

    /// <summary>
    /// Parses the columnar rows into typed <see cref="DestinationRow"/> records.
    /// Expected field order: [arrivalStationId, tripCount, averageDurationSeconds, averageDistanceMetres].
    /// </summary>
    public List<DestinationRow> ParseRows()
    {
        var result = new List<DestinationRow>(Rows.Count);
        foreach (var row in Rows)
        {
            if (row.Count < 4) continue;

            result.Add(new DestinationRow
            {
                ArrivalStationId = ExtractString(row[0]),
                TripCount = ExtractInt(row[1]),
                AverageDurationSeconds = ExtractInt(row[2]),
                AverageDistanceMetres = ExtractInt(row[3])
            });
        }

        result.Sort((a, b) => b.TripCount.CompareTo(a.TripCount));
        return result;
    }

    private static string ExtractString(object? value) => value switch
    {
        string s => s,
        JsonElement je when je.ValueKind == JsonValueKind.String => je.GetString() ?? "",
        _ => value?.ToString() ?? ""
    };

    private static int ExtractInt(object? value) => value switch
    {
        int n => n,
        long l => (int)l,
        double d => (int)d,
        JsonElement je => je.TryGetInt32(out var v) ? v : 0,
        _ => 0
    };
}
