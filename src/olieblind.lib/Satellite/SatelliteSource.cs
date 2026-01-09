using Azure.Messaging.ServiceBus;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Satellite.Interfaces;
using olieblind.lib.Services;
using SixLabors.ImageSharp;
using System.Diagnostics;

namespace olieblind.lib.Satellite;

public class SatelliteSource(IMyRepository repo, IOlieWebService ows, IOlieImageService ois) : ISatelliteSource
{
    public async Task AddInventoryToDatabase(string effectiveDate, string bucket, int channel, DayPartsEnum dayPart, CancellationToken ct)
    {
        var entity = new SatelliteAwsInventoryEntity
        {
            Id = bucket,
            EffectiveDate = effectiveDate,

            Channel = channel,
            DayPart = dayPart,
            Timestamp = DateTime.UtcNow
        };

        await repo.SatelliteAwsInventoryCreate(entity, ct);
    }

    public async Task AddProductsToDatabase(string[] keys, string effectiveDate, string bucket, int channel,
        DayPartsEnum dayPart, Func<string, DateTime> getScanTimeFunc, CancellationToken ct)
    {
        var items = new List<SatelliteAwsProductEntity>();

        foreach (var key in keys)
        {
            var entity = new SatelliteAwsProductEntity
            {
                Id = Path.GetFileName(key),
                EffectiveDate = effectiveDate,

                BucketName = bucket,
                Channel = channel,
                DayPart = dayPart,
                Path1080 = null,
                PathPoster = null,
                PathSource = null,
                ScanTime = getScanTimeFunc(key),
                Timestamp = DateTime.UtcNow,
                TimeTaken1080 = 0,
                TimeTakenDownload = 0,
                TimeTakenPoster = 0
            };

            items.Add(entity);
        }

        await repo.SatelliteAwsProductCreate(items, ct);
    }

    public DateTime GetEffectiveDate(string value)
    {
        var parts = value.Split('-').Select(int.Parse).ToArray();
        var effectiveDate = new DateTime(parts[0], parts[1], parts[2], 0, 0, 0,
            DateTimeKind.Utc);

        return effectiveDate;
    }

    public DateTime GetEffectiveStart(DateTime effectiveDate, DayPartsEnum dayPart)
    {
        return dayPart switch
        {
            DayPartsEnum.Owl => effectiveDate.Date.AddHours(6),
            DayPartsEnum.Morning => effectiveDate.Date.AddHours(12),
            DayPartsEnum.Afternoon => effectiveDate.Date.AddHours(18),
            _ => effectiveDate.Date.AddHours(24)
        };
    }

    public DateTime GetEffectiveStop(DateTime effectiveDate, DayPartsEnum dayPart)
    {
        return dayPart switch
        {
            DayPartsEnum.Owl => effectiveDate.Date.AddHours(11),
            DayPartsEnum.Morning => effectiveDate.Date.AddHours(17),
            DayPartsEnum.Afternoon => effectiveDate.Date.AddHours(23),
            _ => effectiveDate.Date.AddHours(29)
        };
    }

    public string GetPath(DateTime effectiveDate, string metal)
    {
        var pathDate = effectiveDate.ToString("yyyy/MM/dd");
        var path = $"{metal}/aws/satellite/{pathDate}";

        return path;
    }

    public async Task<SatelliteAwsProductEntity?> GetMarqueeSatelliteProduct(string effectiveDate, DateTime eventTime, CancellationToken ct)
    {
        var result =
            await repo.SatelliteAwsProductGetPoster(effectiveDate, eventTime, ct) ??
            await repo.SatelliteAwsProductGetLastPoster(effectiveDate, ct);

        return result;
    }

    public async Task MakeThumbnail(SatelliteAwsProductEntity satellite, Point finalSize, string goldPath, CancellationToken ct)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        // Sanity check
        if (satellite.PathPoster is not null) return;
        if (satellite.Path1080 is null) throw new NullReferenceException($"Missing Path1080 for {satellite.Id}");

        // Download full sized image
        var filename1080 = $"{goldPath}/{satellite.Path1080}";
        var bytes = await ows.FileReadAllBytes(filename1080, ct);

        // Convert to poster image
        var filenamePoster = filename1080.Replace(".png", "_poster.png");
        var finalSizePoint = new System.Drawing.Point(finalSize.X, finalSize.Y);
        var resizedBytes = await ois.Resize(bytes, finalSizePoint, ct);
        await ows.FileWriteAllBytes(filenamePoster, resizedBytes, ct);

        // Update CosmosDb
        satellite.PathPoster = satellite.Path1080.Replace(".png", "_poster.png");
        satellite.Timestamp = DateTime.UtcNow;
        satellite.TimeTakenPoster = (int)stopwatch.Elapsed.TotalSeconds;
        await repo.SatelliteAwsProductUpdate(satellite, ct);
    }

    public async Task MessagePurple(SatelliteAwsProductEntity satellite, ServiceBusSender sender, CancellationToken ct)
    {
        if (satellite.Path1080 is not null || satellite.PathSource is null) return;

        await ows.ServiceBusSendJson(sender, satellite, ct);
    }
}
