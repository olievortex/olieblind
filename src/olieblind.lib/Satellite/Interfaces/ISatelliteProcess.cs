using Amazon.S3;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using olieblind.data.Entities;
using olieblind.data.Enums;
using SixLabors.ImageSharp;

namespace olieblind.lib.Satellite.Interfaces;

public interface ISatelliteProcess
{
    Task CreateThumbnailAndUpdateDailySummary(SatelliteProductEntity satellite, StormEventsDailySummaryEntity summary,
        Point finalSize, string goldPath, CancellationToken ct);

    Task<SatelliteProductEntity?> GetMarqueeSatelliteProduct(StormEventsDailySummaryEntity summary,
        CancellationToken ct);

    Task ProcessMissingDay(int year, string missingDay, int satellite, int channel,
        DayPartsEnum dayPart, IAmazonS3 client, CancellationToken ct);

    Task DownloadSatelliteFile(int year, SatelliteProductEntity satellite, Func<int, Task> delayFunc,
        ServiceBusSender sender, BlobContainerClient blobClient, IAmazonS3 awsClient, CancellationToken ct);

    Task UpdateDailySummary(SatelliteProductEntity satellite, StormEventsDailySummaryEntity summary, CancellationToken ct);
}