using System.Text.Json.Serialization;

namespace HslBikeApp.Models;

public sealed record OpenDataTimeSeries
{
    public const double UnavailableSentinel = -1;

    [JsonPropertyName("sourceId")]
    public required string SourceId { get; init; }

    [JsonPropertyName("displayName")]
    public required string DisplayName { get; init; }

    [JsonPropertyName("lat")]
    public required double Lat { get; init; }

    [JsonPropertyName("lon")]
    public required double Lon { get; init; }

    [JsonPropertyName("attributionUrl")]
    public required string AttributionUrl { get; init; }

    [JsonPropertyName("unit")]
    public string? Unit { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("timestamps")]
    public required IReadOnlyList<DateTimeOffset> Timestamps { get; init; }

    [JsonPropertyName("values")]
    public required IReadOnlyList<double> Values { get; init; }

    public (DateTimeOffset Timestamp, double Value)? LatestAvailable()
    {
        for (var index = Values.Count - 1; index >= 0; index--)
        {
            if (Values[index] != UnavailableSentinel)
                return (Timestamps[index], Values[index]);
        }
        return null;
    }
}
