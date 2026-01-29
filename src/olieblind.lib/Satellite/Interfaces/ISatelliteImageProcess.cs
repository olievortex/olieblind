using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Satellite.Sources;
using SixLabors.ImageSharp;

namespace olieblind.lib.Satellite.Interfaces;

public interface ISatelliteImageProcess
{
    Task CreateThumbnailAndUpdateDailySummary(SatelliteProductEntity product, StormEventsDailySummaryEntity summary, Point finalSize, string goldPath, CancellationToken ct);

    Task<SatelliteProductEntity?> GetMarqueeProduct(StormEventsDailySummaryEntity summary, CancellationToken ct);

    Task DownloadInventory(string effectiveDate, int satellite, int channel, DayPartsEnum dayPart, ASatelliteSource source, CancellationToken ct);
}