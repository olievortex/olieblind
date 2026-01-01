using olieblind.data.Entities;

namespace olieblind.lib.StormEvents.Interfaces;

public interface IDailySummaryBusiness
{
    Task<List<StormEventsDailySummaryEntity>> GetMissingPostersByYear(int year, CancellationToken ct);

    Task<List<StormEventsDailySummaryEntity>> GetSevereByYear(int year, CancellationToken ct);

    Task UpdateCosmos(StormEventsDailySummaryEntity stormSummary, CancellationToken ct);
}