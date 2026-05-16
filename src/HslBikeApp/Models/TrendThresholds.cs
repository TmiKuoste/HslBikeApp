namespace HslBikeApp.Models;

public sealed record TrendThresholds
{
    public static TrendThresholds Default { get; } = new();

    public int ChangeBikes { get; }

    public int RapidChangeBikes { get; }

    /// <summary>Creates bike-count thresholds for classifying availability trends.</summary>
    public TrendThresholds(int ChangeBikes = 1, int RapidChangeBikes = 3)
    {
        if (ChangeBikes < 1)
            throw new ArgumentOutOfRangeException(nameof(ChangeBikes), ChangeBikes, "Change threshold must be at least 1 bike.");

        if (RapidChangeBikes < ChangeBikes)
            throw new ArgumentOutOfRangeException(nameof(RapidChangeBikes), RapidChangeBikes, "Rapid change threshold must be greater than or equal to the change threshold.");

        this.ChangeBikes = ChangeBikes;
        this.RapidChangeBikes = RapidChangeBikes;
    }
}
