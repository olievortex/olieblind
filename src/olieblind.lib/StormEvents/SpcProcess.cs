using olieblind.data.Entities;
using olieblind.lib.StormEvents.Interfaces;
using olieblind.lib.StormEvents.Models;

namespace olieblind.lib.StormEvents;

public class SpcProcess(ISpcBusiness business) : ISpcProcess
{
    //public async Task<(int start, int stop, List<StormEventsSpcInventoryEntity>)> GetInventoryByYearAsync(int year,
    //    CancellationToken ct)
    //{
    //    var start = SpcBusiness.GetFirstDayNumberForYear(year);
    //    var stop = SpcBusiness.GetLastDayNumberForYear(year);
    //    var items =
    //        start <= stop ? await business.GetInventoryByYearAsync(year, ct) : [];

    //    return (start, stop, items);
    //}

    //public async Task ProcessEvents(List<DailyDetailModel> events, StormEventsSpcInventoryEntity inventory,
    //    CancellationToken ct)
    //{
    //    var aggregate = business.GetAggregate(events);

    //    await business.AddDailyDetailAsync(events, inventory, ct);
    //    await business.AddDailySummaryAsync(inventory, aggregate, inventory.Id, ct);
    //}

    //public bool ShouldSkip(StormEventsSpcInventoryEntity inventory)
    //{
    //    return inventory is { IsDailySummaryComplete: true, IsDailyDetailComplete: true };
    //}

    //public async Task<StormEventsSpcInventoryEntity> SourceInventoryAsync(DateTime effectiveDate,
    //    List<StormEventsSpcInventoryEntity> inventoryList, CancellationToken ct)
    //{
    //    var inventory = business.GetLatest(effectiveDate, inventoryList);

    //    if (inventory is null) return await business.DownloadNewAsync(effectiveDate, ct);

    //    return await business.DownloadUpdateAsync(inventory, ct);
    //}
    public Task<(int start, int stop, List<StormEventsReportEntity>)> GetInventoryByYearAsync(int year, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task ProcessEvents(List<DailyDetailModel> events, StormEventsReportEntity inventory, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public bool ShouldSkip(StormEventsReportEntity inventory)
    {
        throw new NotImplementedException();
    }

    public Task<StormEventsReportEntity> SourceInventoryAsync(DateTime effectiveDate, List<StormEventsReportEntity> inventoryList, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}