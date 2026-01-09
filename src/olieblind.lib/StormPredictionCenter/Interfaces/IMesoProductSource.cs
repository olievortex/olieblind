using Azure.Storage.Blobs;
using olieblind.data.Entities;

namespace olieblind.lib.StormPredictionCenter.Interfaces;

public interface IMesoProductSource
{
    Task<string?> DownloadHtml(int year, int index, CancellationToken ct);

    Task<string?> StoreImage(string? imageName, SpcMesoProductEntity product, string goldPath, CancellationToken ct);

    Task<string> StoreHtml(int index, string html, DateTime effectiveDate, BlobContainerClient bronze, CancellationToken ct);

    Task<int> GetLatestIdForYear(int year, CancellationToken ct);
}