using Azure.Storage.Blobs;

namespace olieblind.lib.StormPredictionCenter.Interfaces;

public interface IMesoProductProcess
{
    Task<bool> Download(int year, int index, string goldPath, BlobContainerClient bronze, CancellationToken ct);
}