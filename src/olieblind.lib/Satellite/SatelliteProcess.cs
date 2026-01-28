using Amazon.S3;
using Azure.Messaging.ServiceBus;
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

public class SatelliteProcess(ISatelliteImageBusiness business, IMyRepository repo, IOlieWebService ows) : ISatelliteProcess
{
    // GOES 16 became operational during December 2017
    private const int Goes16 = 2018;

    public ASatelliteSource CreateSatelliteSource(int year, IAmazonS3 amazonS3Client)
    {
        return year < Goes16 ?
            new SatelliteIemSource { Ows = ows } :
            new SatelliteAwsSource { AmazonS3Client = amazonS3Client, Ows = ows };
    }

    public async Task CreateThumbnailAndUpdateDailySummary(SatelliteProductEntity product, StormEventsDailySummaryEntity summary, Point finalSize, string goldPath, CancellationToken ct)
    {
        if (summary.SatellitePath1080 is not null && summary.SatellitePathPoster is null)
        {
            if (product.PathPoster is null)
                await business.MakeThumbnail(product, finalSize, goldPath, ct);

            summary.SatellitePathPoster = product.PathPoster;
            summary.Timestamp = DateTime.UtcNow;
            await repo.StormEventsDailySummaryUpdate(summary, ct);
        }
    }

    public async Task<SatelliteProductEntity?> GetMarqueeSatelliteProduct(StormEventsDailySummaryEntity summary, CancellationToken ct)
    {
        if (summary.HeadlineEventTime is null) return null;
        if (summary.SatellitePathPoster is not null && summary.SatellitePath1080 is not null) return null;

        var satellite = await business.GetMarqueeSatelliteProduct(summary.Id, summary.HeadlineEventTime.Value, ct);

        return satellite;
    }

    public async Task DownloadInventory(string effectiveDate, int satellite, int channel, DayPartsEnum dayPart, ASatelliteSource source, CancellationToken ct)
    {
        var result = await source.ListKeys(effectiveDate, satellite, channel, dayPart, ct);
        if (result is null || result.Keys.Length == 0) return;

        await business.AddProductsToDatabase(result.Keys, effectiveDate, result.Bucket, channel, dayPart, result.GetScanTimeFunc, ct);
        await business.AddInventoryToDatabase(effectiveDate, result.Bucket, channel, dayPart, ct);
    }

    public async Task DownloadSatelliteFile(SatelliteProductEntity product, ServiceBusSender sender, BlobContainerClient blobClient, ASatelliteSource source, CancellationToken ct)
    {
        string? blobName, localFilename;

        if (product.PathSource is not null) return;

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        (blobName, localFilename) = await source.Download(product, CreateDelayFunc(ct), ct);

        await ows.BlobUploadFile(blobClient, blobName, localFilename, ct);

        product.PathLocal = localFilename;
        product.PathSource = blobName;
        product.TimeTakenDownload = (int)stopwatch.Elapsed.TotalSeconds;
        product.Timestamp = DateTime.UtcNow;
        await repo.SatelliteProductUpdate(product, ct);
    }

    public async Task UpdateDailySummary(SatelliteProductEntity product, StormEventsDailySummaryEntity summary, CancellationToken ct)
    {
        if (summary.SatellitePath1080 is null && product.Path1080 is not null)
        {
            summary.SatellitePath1080 = product.Path1080;
            summary.Timestamp = DateTime.UtcNow;
            await repo.StormEventsDailySummaryUpdate(summary, ct);
        }
    }

    private static Func<int, Task> CreateDelayFunc(CancellationToken ct)
    {
        return (async attempt =>
        {
            await Task.Delay(30 * attempt, ct);
        });
    }
}
