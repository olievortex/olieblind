using Amazon.S3;
using Azure.Storage.Blobs;
using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Satellite.Sources;
using olieblind.lib.Services;
using SixLabors.ImageSharp;

namespace olieblind.lib.Satellite.Interfaces;

public interface ISatelliteImageBusiness
{
    Task AddInventoryToDatabase(string effectiveDate, string bucket, int channel, DayPartsEnum dayPart, CancellationToken ct);

    Task AddProductsToDatabase(string[] keys, string effectiveDate, string bucket, int channel, DayPartsEnum dayPart, Func<string, DateTime> scanTimeFunc, CancellationToken ct);

    ASatelliteSource CreateSatelliteSource(int year, IAmazonS3 amazonS3Client);

    Task DownloadProduct(SatelliteProductEntity product, ASatelliteSource source, BlobContainerClient blobClient, CancellationToken ct);

    Task<SatelliteProductEntity?> GetMarqueeProduct(StormEventsDailySummaryEntity summary, CancellationToken ct);

    Task Make1080(SatelliteProductEntity product, IOlieConfig config, CancellationToken ct);

    Task MakePoster(SatelliteProductEntity product, Point finalSize, string goldPath, CancellationToken ct);

    Task UpdateDailySummary1080(SatelliteProductEntity product, StormEventsDailySummaryEntity summary, CancellationToken ct);

    Task UpdateDailySummaryPoster(SatelliteProductEntity product, StormEventsDailySummaryEntity summary, CancellationToken ct);
}
