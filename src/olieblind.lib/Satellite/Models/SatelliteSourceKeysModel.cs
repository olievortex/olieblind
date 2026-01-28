namespace olieblind.lib.Satellite.Models;

public class SatelliteSourceKeysModel
{
    public string Bucket { get; init; } = string.Empty;
    public string[] Keys { get; init; } = [];
    public required Func<string, DateTime> GetScanTimeFunc { get; init; }
}