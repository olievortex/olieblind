using olieblind.data.Entities;

namespace olieblind.lib.StormEvents.Models;

public class DailyOverviewModel
{
    public List<StormEventsDailyDetailEntity> Events { get; init; } = [];
    public List<StormEventsDailyDetailEntity> Tornadoes { get; init; } = [];
    public List<StormEventsDailyDetailEntity> Hails { get; init; } = [];
    public List<StormEventsDailyDetailEntity> Winds { get; init; } = [];
    public int MesoCount { get; init; }
    public string? Satellite1080Path { get; init; }
    public DateTime? SatelliteDateTime { get; init; }
    public string SatelliteAttribution { get; init; } = string.Empty;
    public string? SatellitePosterPath { get; init; }
}