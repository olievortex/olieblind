namespace olieblind.lib.DroughtMonitor.Models;

public class DroughtMonitorProductModel
{
    public DateTime EffectiveDate { get; init; }
    public string Intro { get; init; } = string.Empty;
    public string Forecast { get; init; } = string.Empty;
    public Dictionary<string, string> Regions { get; init; } = [];
}