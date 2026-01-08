using Azure.Storage.Blobs;
using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Satellite.Models;

namespace olieblind.lib.Satellite.Interfaces;

public interface ISatelliteIemBusiness
{
    Task DownloadAsync(SatelliteAwsProductEntity product, Func<int, Task> delayFunc,
        BlobContainerClient blobClient, CancellationToken ct);

    Task<AwsKeysModel?> ListKeysAsync(string dayValue, int channel, DayPartsEnum dayPart, CancellationToken ct);
}