using CsvHelper.Configuration.Attributes;

namespace olieblind.lib.StormEvents.Models;

public class StormEventRowModel
{
    [Name("STATE")] public string State { get; init; } = string.Empty;
    [Name("CZ_NAME")] public string County { get; init; } = string.Empty;
    [Name("BEGIN_LOCATION")] public string City { get; init; } = string.Empty;
    [Name("EVENT_TYPE")] public string EventType { get; init; } = string.Empty;
    [Name("WFO")] public string ForecastOffice { get; init; } = string.Empty;
    [Name("BEGIN_DATE_TIME")] public string Effective { get; init; } = string.Empty;
    [Name("CZ_TIMEZONE")] public string TimeZone { get; init; } = string.Empty;
    [Name("MAGNITUDE")]
    public double? Magnitude { get; init; }
    [Name("TOR_F_SCALE")] public string TornadoFScale { get; init; } = string.Empty;
    [Name("BEGIN_LAT")]
    public float? Latitude { get; init; }
    [Name("BEGIN_LON")] public float? Longitude { get; init; }
    [Name("EVENT_NARRATIVE")] public string Narrative { get; init; } = string.Empty;
}
