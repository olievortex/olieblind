using Amazon.S3;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Satellite.Sources;
using SixLabors.ImageSharp;

namespace olieblind.lib.Satellite.Interfaces;

public interface ISatelliteProcess
{
    ASatelliteSource CreateSatelliteSource(int year, IAmazonS3 amazonS3Client);

    Task CreateThumbnailAndUpdateDailySummary(SatelliteProductEntity product, StormEventsDailySummaryEntity summary, Point finalSize, string goldPath, CancellationToken ct);

    Task<SatelliteProductEntity?> GetMarqueeSatelliteProduct(StormEventsDailySummaryEntity summary, CancellationToken ct);

    Task DownloadInventory(string effectiveDate, int satellite, int channel, DayPartsEnum dayPart, ASatelliteSource source, CancellationToken ct);

    Task DownloadSatelliteFile(SatelliteProductEntity product, ServiceBusSender sender, BlobContainerClient blobClient, ASatelliteSource source, CancellationToken ct);

    Task UpdateDailySummary(SatelliteProductEntity product, StormEventsDailySummaryEntity summary, CancellationToken ct);
}