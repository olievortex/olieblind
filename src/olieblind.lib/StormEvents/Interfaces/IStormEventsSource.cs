using olieblind.data.Entities;
using olieblind.data.Models;

namespace olieblind.lib.StormEvents.Interfaces;

public interface IStormEventsSource
{
    DateTime? FromEffectiveDate(string value);

    Task<List<StormEventsAnnualSummaryModel>> GetAnnualSummaryList(CancellationToken ct);

    Task<StormEventsDailySummaryEntity?> GetDailySummaryByDate(string effectiveDate, int year, CancellationToken ct);

    Task<List<StormEventsDailySummaryEntity>> GetDailySummaryList(int year, CancellationToken ct);
}
