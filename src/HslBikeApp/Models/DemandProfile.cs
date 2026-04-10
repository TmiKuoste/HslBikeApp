namespace HslBikeApp.Models;

/// <summary>
/// Hourly demand profile from the monthly statistics endpoint.
/// Each array has exactly 24 elements (hours 0–23).
/// </summary>
public record DemandProfile
{
    public int[] DeparturesByHour { get; init; } = [];
    public int[] ArrivalsByHour { get; init; } = [];
    public int[] WeekdayDeparturesByHour { get; init; } = [];
    public int[] WeekendDeparturesByHour { get; init; } = [];
    public int[] WeekdayArrivalsByHour { get; init; } = [];
    public int[] WeekendArrivalsByHour { get; init; } = [];
}
