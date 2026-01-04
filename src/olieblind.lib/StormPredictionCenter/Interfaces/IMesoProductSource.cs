using olieblind.data.Entities;

namespace olieblind.lib.StormPredictionCenter.Interfaces;

public interface IMesoProductSource
{
    Task<string?> DownloadHtml(int year, int index, CancellationToken ct);

    Task DownloadImage(string imageName, SpcMesoProductEntity product, string goldPath, CancellationToken ct);

    Task<int> GetLatestIdForYear(int year, CancellationToken ct);
}