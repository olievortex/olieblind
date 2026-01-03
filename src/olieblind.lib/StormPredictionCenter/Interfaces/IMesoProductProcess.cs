using Azure.Storage.Blobs;

namespace olieblind.lib.StormPredictionCenter.Interfaces;

public interface IMesoProductProcess
{
    Task<bool> Download(int year, int index, string goldPath, CancellationToken ct);
    Task<bool> Update(int year, int index, CancellationToken ct);
}