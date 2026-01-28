using Amazon.S3;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Satellite.Interfaces;
using olieblind.lib.Services;
using SixLabors.ImageSharp;
using System.Diagnostics;

namespace olieblind.lib.Satellite;

public class SatelliteProcess(ASatelliteSource awsSource, ASatelliteSource iemSource,
    ISatelliteSource source, IMyRepository repo, IOlieWebService ows) : ISatelliteProcess
{
    // GOES 16 became operational during December 2017
    private const int Goes16 = 2018;

    public async Task CreateThumbnailAndUpdateDailySummary(SatelliteProductEntity satellite, StormEventsDailySummaryEntity summary,
        Point finalSize, string goldPath, CancellationToken ct)
    {
        if (summary.SatellitePath1080 is not null && summary.SatellitePathPoster is null)
        {
            if (satellite.PathPoster is null)
                await source.MakeThumbnail(satellite, finalSize, goldPath, ct);

            summary.SatellitePathPoster = satellite.PathPoster;
            summary.Timestamp = DateTime.UtcNow;
            await repo.StormEventsDailySummaryUpdate(summary, ct);
        }
    }

    public async Task<SatelliteProductEntity?> GetMarqueeSatelliteProduct(StormEventsDailySummaryEntity summary, CancellationToken ct)
    {
        if (summary.HeadlineEventTime is null) return null;
        if (summary.SatellitePathPoster is not null && summary.SatellitePath1080 is not null) return null;

        var satellite = await source.GetMarqueeSatelliteProduct(summary.Id, summary.HeadlineEventTime.Value, ct);

        return satellite;
    }

    public async Task ProcessMissingDay(int year, string missingDay, int satellite, int channel,
        DayPartsEnum dayPart, IAmazonS3 client, CancellationToken ct)
    {
        var source = year < Goes16 ? iemSource : awsSource;

        var result = await source.ListKeys(missingDay, satellite, channel, dayPart, ct);
        if (result is null || result.Keys.Length == 0) return;

        await source.AddProductsToDatabase(result.Keys, missingDay, result.Bucket, channel, dayPart, result.GetScanTimeFunc, ct);
        await source.AddInventoryToDatabase(missingDay, result.Bucket, channel, dayPart, ct);
    }

    public async Task DownloadSatelliteFile(int year, SatelliteProductEntity satellite, Func<int, Task> delayFunc,
        ServiceBusSender sender, BlobContainerClient blobClient, IAmazonS3 awsClient, CancellationToken ct)
    {
        string? blobName, localFilename;

        if (satellite.PathSource is not null) return;

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        if (year < Goes16)
            (blobName, localFilename) = await iemSource.Download(satellite, ct);
        else
            (blobName, localFilename) = await awsSource.Download(satellite, ct);

        await ows.BlobUploadFile(blobClient, blobName, localFilename, ct);

        satellite.PathLocal = localFilename;
        satellite.PathSource = blobName;
        satellite.TimeTakenDownload = (int)stopwatch.Elapsed.TotalSeconds;
        satellite.Timestamp = DateTime.UtcNow;
        await repo.SatelliteProductUpdate(satellite, ct);

        await source.MessagePurple(satellite, sender, ct);
    }

    public async Task UpdateDailySummary(SatelliteProductEntity satellite, StormEventsDailySummaryEntity summary, CancellationToken ct)
    {
        if (summary.SatellitePath1080 is null && satellite.Path1080 is not null)
        {
            summary.SatellitePath1080 = satellite.Path1080;
            summary.Timestamp = DateTime.UtcNow;
            await repo.StormEventsDailySummaryUpdate(summary, ct);
        }
    }
}
