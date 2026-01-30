using Amazon.S3;
using Azure.Storage.Blobs;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Satellite.Interfaces;
using olieblind.lib.Satellite.Sources;
using olieblind.lib.Services;
using SixLabors.ImageSharp;
using System.Diagnostics;

namespace olieblind.lib.Satellite;

public class SatelliteImageBusiness(IOlieWebService ows, IOlieImageService ois, IMyRepository repo) : ISatelliteImageBusiness
{
    // GOES 16 became operational during December 2017
    private const int Goes16 = 2018;

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

    public static Func<int, Task> CreateDelayFunc(CancellationToken ct)
    {
        return (async attempt =>
        {
            await Task.Delay(30 * attempt, ct);
        });
    }

    public ASatelliteSource CreateSatelliteSource(int year, IAmazonS3 amazonS3Client)
    {
        return year < Goes16 ?
            new SatelliteIemSource { Ows = ows } :
            new SatelliteAwsSource { AmazonS3Client = amazonS3Client, Ows = ows };
    }

    public async Task DownloadProduct(SatelliteProductEntity product, ASatelliteSource source, BlobContainerClient blobClient, CancellationToken ct)
    {
        string? blobName, localFilename;

        // Sanity check
        if (product.PathSource is not null) return;

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        (blobName, localFilename) = await source.Download(product, CreateDelayFunc(ct), ct);

        await ows.BlobUploadFile(blobClient, blobName, localFilename, ct);

        // Update database
        product.PathLocal = localFilename;
        product.PathSource = blobName;
        product.TimeTakenDownload = (int)stopwatch.Elapsed.TotalSeconds;
        product.Timestamp = DateTime.UtcNow;
        await repo.SatelliteProductUpdate(product, ct);
    }

    public async Task<SatelliteProductEntity?> GetMarqueeProduct(StormEventsDailySummaryEntity summary, CancellationToken ct)
    {
        if (summary.HeadlineEventTime is null) return null;
        if (summary.SatellitePathPoster is not null && summary.SatellitePath1080 is not null) return null;

        var result =
            await repo.SatelliteProductGetPoster(summary.Id, summary.HeadlineEventTime.Value, ct) ??
            await repo.SatelliteProductGetLastPoster(summary.Id, ct);

        return result;
    }

    public async Task Make1080(SatelliteProductEntity product, IOlieConfig config, CancellationToken ct)
    {
        const string pythonScript = "olievortex_purple_nc_2_png.py";

        if (product.Path1080 is not null) return;

        var args = $"{pythonScript} {product.Id} {product.EffectiveDate}";

        await ows.Shell(config, config.PurpleCmdPath, args, ct);
    }

    public async Task MakePoster(SatelliteProductEntity product, Point finalSize, string goldPath, CancellationToken ct)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        // Sanity check
        if (product.PathPoster is not null) return;
        if (product.Path1080 is null) throw new NullReferenceException($"Missing Path1080 for {product.Id}");

        // Download full sized image
        var filename1080 = $"{goldPath}/{product.Path1080}";
        var bytes = await ows.FileReadAllBytes(filename1080, ct);

        // Convert to poster image
        var filenamePoster = filename1080.Replace(".png", "_poster.png");
        var finalSizePoint = new System.Drawing.Point(finalSize.X, finalSize.Y);
        var resizedBytes = await ois.Resize(bytes, finalSizePoint, ct);
        await ows.FileWriteAllBytes(filenamePoster, resizedBytes, ct);

        // Update Product
        product.PathPoster = product.Path1080.Replace(".png", "_poster.png");
        product.Timestamp = DateTime.UtcNow;
        product.TimeTakenPoster = (int)stopwatch.Elapsed.TotalSeconds;
        await repo.SatelliteProductUpdate(product, ct);
    }

    public async Task UpdateDailySummary1080(SatelliteProductEntity product, StormEventsDailySummaryEntity summary, CancellationToken ct)
    {
        if (summary.SatellitePath1080 is null && product.Path1080 is not null)
        {
            summary.SatellitePath1080 = product.Path1080;
            summary.Timestamp = DateTime.UtcNow;
            await repo.StormEventsDailySummaryUpdate(summary, ct);
        }
    }

    public async Task UpdateDailySummaryPoster(SatelliteProductEntity product, StormEventsDailySummaryEntity summary, CancellationToken ct)
    {
        if (summary.SatellitePathPoster is null && product.PathPoster is not null)
        {
            summary.SatellitePathPoster = product.PathPoster;
            summary.Timestamp = DateTime.UtcNow;
            await repo.StormEventsDailySummaryUpdate(summary, ct);
        }
    }
}
