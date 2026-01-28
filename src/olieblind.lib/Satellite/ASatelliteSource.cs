using olieblind.data;
using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Satellite.Models;

namespace olieblind.lib.Satellite;

public abstract class ASatelliteSource
{
    public required Func<int, Task> DelayFunc { get; init; }
    public required IMyRepository Repository { get; init; }

    public abstract Task<(string, string)> Download(SatelliteProductEntity product, CancellationToken ct);

    public abstract Task<SatelliteSourceKeysModel?> ListKeys(string dayValue, int satellite, int channel, DayPartsEnum dayPart, CancellationToken ct);

    public async Task AddInventoryToDatabase(string effectiveDate, string bucket, int channel, DayPartsEnum dayPart, CancellationToken ct)
    {
        var entity = new SatelliteInventoryEntity
        {
            Id = bucket,
            EffectiveDate = effectiveDate,

            Channel = channel,
            DayPart = dayPart,
            Timestamp = DateTime.UtcNow
        };

        await Repository.SatelliteInventoryCreate(entity, ct);
    }

    public async Task AddProductsToDatabase(string[] keys, string effectiveDate, string bucket, int channel, DayPartsEnum dayPart,
        Func<string, DateTime> scanTimeFunc, CancellationToken ct)
    {
        var items = new List<SatelliteProductEntity>();

        foreach (var key in keys)
        {
            var entity = new SatelliteProductEntity
            {
                Id = Path.GetFileName(key),
                EffectiveDate = effectiveDate,

                BucketName = bucket,
                Channel = channel,
                DayPart = dayPart,
                Path1080 = null,
                PathPoster = null,
                PathSource = null,
                ScanTime = scanTimeFunc(key),
                Timestamp = DateTime.UtcNow,
                TimeTaken1080 = 0,
                TimeTakenDownload = 0,
                TimeTakenPoster = 0
            };

            items.Add(entity);
        }

        await Repository.SatelliteProductCreate(items, ct);
    }


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
