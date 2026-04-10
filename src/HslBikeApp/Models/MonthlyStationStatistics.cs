namespace HslBikeApp.Models;

/// <summary>
/// Response from GET /api/stations/{id}/statistics.
/// Contains demand profile and destination table for a given month.
/// </summary>
public record MonthlyStationStatistics
{
    public string Month { get; init; } = "";
    public DemandProfile Demand { get; init; } = new();
    public DestinationTable Destinations { get; init; } = new();
}
