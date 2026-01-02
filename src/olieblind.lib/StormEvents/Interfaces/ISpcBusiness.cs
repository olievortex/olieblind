using olieblind.data.Entities;
using olieblind.lib.StormEvents.Models;

namespace olieblind.lib.StormEvents.Interfaces;

public interface ISpcBusiness
{
    //Task AddDailyDetailAsync(List<DailyDetailModel> models, StormEventsReportEntity inventory, CancellationToken ct);

    //Task AddDailySummaryAsync(StormEventsReportEntity inventory, DailySummaryModel? model, string sourceFk, CancellationToken ct);

    //Task<StormEventsReportEntity> DownloadNewAsync(DateTime effectiveDate, CancellationToken ct);

    //Task<StormEventsReportEntity> DownloadUpdateAsync(StormEventsReportEntity inventory, CancellationToken ct);

    //DailySummaryModel? GetAggregate(List<DailyDetailModel> models);

    List<DailyDetailModel> Parse(DateTime effectiveDate, string[] lines);

    //Task<List<StormEventsReportEntity>> GetInventoryByYearAsync(int year, CancellationToken ct);

    //StormEventsReportEntity? GetLatest(DateTime effectiveDate, List<StormEventsReportEntity> inventory);
}