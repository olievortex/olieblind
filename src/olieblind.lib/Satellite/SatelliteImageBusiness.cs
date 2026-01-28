using olieblind.data;
using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Satellite.Interfaces;
using olieblind.lib.Services;
using SixLabors.ImageSharp;
using System.Diagnostics;

namespace olieblind.lib.Satellite;

public class SatelliteImageBusiness(IOlieWebService ows, IOlieImageService ois, IMyRepository repo) : ISatelliteImageBusiness
{
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

        await repo.SatelliteInventoryCreate(entity, ct);
    }

    public async Task AddProductsToDatabase(string[] keys, string effectiveDate, string bucket, int channel, DayPartsEnum dayPart, Func<string, DateTime> scanTimeFunc, CancellationToken ct)
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

        await repo.SatelliteProductCreate(items, ct);
    }

    public async Task<SatelliteProductEntity?> GetMarqueeSatelliteProduct(string effectiveDate, DateTime eventTime, CancellationToken ct)
    {
        var result =
            await repo.SatelliteProductGetPoster(effectiveDate, eventTime, ct) ??
            await repo.SatelliteProductGetLastPoster(effectiveDate, ct);

        return result;
    }

    public async Task MakeThumbnail(SatelliteProductEntity satellite, Point finalSize, string goldPath, CancellationToken ct)
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
        await repo.SatelliteProductUpdate(satellite, ct);
    }
}
