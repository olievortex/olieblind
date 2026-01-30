using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Satellite.Models;
using olieblind.lib.Services;
using SixLabors.ImageSharp;

namespace olieblind.lib.Satellite.Sources;

public abstract class ASatelliteSource
{
    public required IOlieWebService Ows { get; init; }

    public abstract Task<(string, string)> Download(SatelliteProductEntity product, Func<int, Task> delayFunc, CancellationToken ct);

    public abstract Task<SatelliteSourceKeysModel?> ListKeys(string dayValue, int satellite, int channel, DayPartsEnum dayPart, CancellationToken ct);

    public static DateTime GetEffectiveDate(string value)
    {
        var parts = value.Split('-').Select(int.Parse).ToArray();
        var effectiveDate = new DateTime(parts[0], parts[1], parts[2], 0, 0, 0,
            DateTimeKind.Utc);

        return effectiveDate;
    }

    public static DateTime GetEffectiveStart(DateTime effectiveDate, DayPartsEnum dayPart)
    {
        return dayPart switch
        {
            DayPartsEnum.Owl => effectiveDate.Date.AddHours(6),
            DayPartsEnum.Morning => effectiveDate.Date.AddHours(12),
            DayPartsEnum.Afternoon => effectiveDate.Date.AddHours(18),
            _ => effectiveDate.Date.AddHours(24)
        };
    }

    public static DateTime GetEffectiveStop(DateTime effectiveDate, DayPartsEnum dayPart)
    {
        return dayPart switch
        {
            DayPartsEnum.Owl => effectiveDate.Date.AddHours(11),
            DayPartsEnum.Morning => effectiveDate.Date.AddHours(17),
            DayPartsEnum.Afternoon => effectiveDate.Date.AddHours(23),
            _ => effectiveDate.Date.AddHours(29)
        };
    }

    public static string GetPath(DateTime effectiveDate, string metal)
    {
        var pathDate = effectiveDate.ToString("yyyy/MM/dd");
        var path = $"{metal}/aws/satellite/{pathDate}";

        return path;
    }
}
