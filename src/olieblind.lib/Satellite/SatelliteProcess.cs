using Amazon.S3;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Satellite.Interfaces;
using SixLabors.ImageSharp;

namespace olieblind.lib.Satellite;

public class SatelliteProcess(ISatelliteAwsBusiness awsBusiness, ISatelliteIemBusiness iemBusiness,
    ISatelliteSource source, IMyRepository repo) : ISatelliteProcess
{
    // GOES 16 became operational during December 2017
    private const int Goes16 = 2018;

    public async Task CreateThumbnailAndUpdateDailySummary(SatelliteAwsProductEntity satellite, StormEventsDailySummaryEntity summary,
        Point finalSize, BlobContainerClient goldClient, CancellationToken ct)
    {
        if (summary.SatellitePath1080 is not null && summary.SatellitePathPoster is null)
        {
            if (satellite.PathPoster is null)
                await source.MakeThumbnail(satellite, finalSize, goldClient, ct);

            summary.SatellitePathPoster = satellite.PathPoster;
            await repo.StormEventsDailySummaryUpdate(summary, ct);
        }
    }

    public async Task<SatelliteAwsProductEntity?> GetMarqueeSatelliteProduct(StormEventsDailySummaryEntity summary, CancellationToken ct)
    {
        if (summary.HeadlineEventTime is null) return null;
        if (summary.SatellitePathPoster is not null && summary.SatellitePath1080 is not null) return null;

        var satellite = await source.GetMarqueeSatelliteProduct(summary.Id, summary.HeadlineEventTime.Value, ct);

        return satellite;
    }

    public async Task ProcessMissingDay(int year, string missingDay, int satellite, int channel,
        DayPartsEnum dayPart, IAmazonS3 client, CancellationToken ct)
    {
        var result = year < Goes16
            ? await iemBusiness.ListKeys(missingDay, channel, dayPart, ct)
            : await awsBusiness.ListKeys(missingDay, satellite, channel, dayPart, client, ct);
        if (result is null || result.Keys.Length == 0) return;

        await source.AddProductsToDatabase(result.Keys, missingDay, result.Bucket, channel, dayPart,
            result.GetScanTimeFunc, ct);
        await source.AddInventoryToDatabase(missingDay, result.Bucket, channel, dayPart, ct);
    }

    public async Task DownloadSatelliteFile(int year, SatelliteAwsProductEntity satellite, Func<int, Task> delayFunc,
        ServiceBusSender sender, BlobContainerClient blobClient, IAmazonS3 awsClient, CancellationToken ct)
    {
        if (year < Goes16)
            await iemBusiness.Download(satellite, delayFunc, blobClient, ct);
        else
            await awsBusiness.Download(satellite, delayFunc, blobClient, awsClient, ct);

        await source.MessagePurple(satellite, sender, ct);
    }

    public async Task UpdateDailySummary(SatelliteAwsProductEntity satellite, StormEventsDailySummaryEntity summary, CancellationToken ct)
    {
        if (summary.SatellitePath1080 is null && satellite.Path1080 is not null)
        {
            summary.SatellitePath1080 = satellite.Path1080;
            summary.Timestamp = DateTime.UtcNow;
            await repo.StormEventsDailySummaryUpdate(summary, ct);
        }
    }
}
