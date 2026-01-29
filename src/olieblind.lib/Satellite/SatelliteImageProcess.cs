using olieblind.data;
using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Satellite.Interfaces;
using olieblind.lib.Satellite.Sources;
using SixLabors.ImageSharp;

namespace olieblind.lib.Satellite;

public class SatelliteImageProcess(ISatelliteImageBusiness business, IMyRepository repo) : ISatelliteImageProcess
{
    public async Task CreateThumbnailAndUpdateDailySummary(SatelliteProductEntity product, StormEventsDailySummaryEntity summary, Point finalSize, string goldPath, CancellationToken ct)
    {
        if (summary.SatellitePath1080 is not null && summary.SatellitePathPoster is null)
        {
            if (product.PathPoster is null)
                await business.MakeThumbnail(product, finalSize, goldPath, ct);

            summary.SatellitePathPoster = product.PathPoster;
            summary.Timestamp = DateTime.UtcNow;
            await repo.StormEventsDailySummaryUpdate(summary, ct);
        }
    }

    public async Task<SatelliteProductEntity?> GetMarqueeProduct(StormEventsDailySummaryEntity summary, CancellationToken ct)
    {
        if (summary.HeadlineEventTime is null) return null;
        if (summary.SatellitePathPoster is not null && summary.SatellitePath1080 is not null) return null;

        var satellite = await business.GetMarqueeProduct(summary.Id, summary.HeadlineEventTime.Value, ct);

        return satellite;
    }

    public async Task DownloadInventory(string effectiveDate, int satellite, int channel, DayPartsEnum dayPart, ASatelliteSource source, CancellationToken ct)
    {
        var result = await source.ListKeys(effectiveDate, satellite, channel, dayPart, ct);
        if (result is null || result.Keys.Length == 0) return;

        await business.AddProductsToDatabase(result.Keys, effectiveDate, result.Bucket, channel, dayPart, result.GetScanTimeFunc, ct);
        await business.AddInventoryToDatabase(effectiveDate, result.Bucket, channel, dayPart, ct);
    }
}
