using Amazon.S3;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using olieblind.data.Entities;
using olieblind.data.Enums;
using SixLabors.ImageSharp;

namespace olieblind.lib.Satellite.Interfaces;

public interface ISatelliteProcess
{
    Task CreatePoster(SatelliteAwsProductEntity satellite, StormEventsDailySummaryEntity summary,
        Point finalSize, BlobContainerClient goldClient, CancellationToken ct);

    Task<SatelliteAwsProductEntity?> GetMarqueeSatelliteProduct(StormEventsDailySummaryEntity summary,
        CancellationToken ct);

    Task ProcessMissingDay(int year, string missingDay, int satellite, int channel,
        DayPartsEnum dayPart, IAmazonS3 client, CancellationToken ct);

    Task Source1080(int year, SatelliteAwsProductEntity satellite, Func<int, Task> delayFunc,
        ServiceBusSender sender, BlobContainerClient blobClient, IAmazonS3 awsClient, CancellationToken ct);

    Task Update1080(SatelliteAwsProductEntity satellite, StormEventsDailySummaryEntity summary,
        CancellationToken ct);
}