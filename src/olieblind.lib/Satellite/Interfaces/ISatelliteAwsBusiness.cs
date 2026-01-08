using Amazon.S3;
using Azure.Storage.Blobs;
using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Satellite.Models;

namespace olieblind.lib.Satellite.Interfaces;

public interface ISatelliteAwsBusiness
{
    Task DownloadAsync(SatelliteAwsProductEntity product, Func<int, Task> delayFunc, BlobContainerClient blobClient,
        IAmazonS3 awsClient, CancellationToken ct);

    Task<AwsKeysModel?> ListKeys(string dayValue, int satellite, int channel, DayPartsEnum dayPart,
        IAmazonS3 client, CancellationToken ct);
}