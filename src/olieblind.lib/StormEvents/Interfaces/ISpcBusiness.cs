using olieblind.data.Entities;
using olieblind.lib.StormEvents.Models;

namespace olieblind.lib.StormEvents.Interfaces;

public interface ISpcBusiness
{
    Task AddDailyDetail(List<DailyDetailModel> models, StormEventsReportEntity inventory, CancellationToken ct);

    Task AddDailySummary(StormEventsReportEntity inventory, DailySummaryModel? model, string sourceFk, CancellationToken ct);

    Task<StormEventsReportEntity> DownloadNew(DateTime effectiveDate, CancellationToken ct);

    Task<StormEventsReportEntity> DownloadUpdate(StormEventsReportEntity inventory, CancellationToken ct);

    DailySummaryModel? GetAggregate(List<DailyDetailModel> models);

    StormEventsReportEntity? GetLatest(DateTime effectiveDate, List<StormEventsReportEntity> inventory);
}