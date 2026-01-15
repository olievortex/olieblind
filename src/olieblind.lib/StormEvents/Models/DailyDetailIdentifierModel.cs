namespace olieblind.lib.StormEvents.Models;

public class DailyDetailIdentifierModel
{
    public string SourceFk { get; init; } = string.Empty;
    public string EffectiveDate { get; init; } = string.Empty;
    public int Year { get; init; }
}