namespace olieblind.lib.Satellite.Models;

public class SatelliteRequestStatisticsModel
{
    public int GlobalRequests { get; init; }
    public int UserRequests { get; init; }
    public int QueueLength { get; init; }
}
