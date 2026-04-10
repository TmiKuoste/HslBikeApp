namespace HslBikeApp.Models;

/// <summary>
/// A single destination from the statistics endpoint's columnar destinations table.
/// </summary>
public record DestinationRow
{
    public required string ArrivalStationId { get; init; }
    public int TripCount { get; init; }
    public int AverageDurationSeconds { get; init; }
    public int AverageDistanceMetres { get; init; }

    public string AverageDurationFormatted
    {
        get
        {
            var minutes = AverageDurationSeconds / 60;
            return minutes < 1 ? "<1 min" : $"{minutes} min";
        }
    }
}
