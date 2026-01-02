using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.StormEvents.Interfaces;
using olieblind.lib.StormEvents.Models;

namespace olieblind.lib.StormEvents;

public class SpcProcess(ISpcBusiness business, IMyRepository repo) : ISpcProcess
{
    public async Task<(int start, int stop, List<StormEventsReportEntity>)> GetInventoryByYear(int year, CancellationToken ct)
    {
        var start = SpcBusiness.GetFirstDayNumberForYear(year);
        var stop = SpcBusiness.GetLastDayNumberForYear(year);
        var items =
            start <= stop ? await repo.StormEventsReportsByYear(year, ct) : [];

        return (start, stop, items);
    }

    public async Task ProcessEvents(List<DailyDetailModel> events, StormEventsReportEntity inventory, CancellationToken ct)
    {
        var aggregate = business.GetAggregate(events);

        await business.AddDailyDetail(events, inventory, ct);
        await business.AddDailySummary(inventory, aggregate, inventory.Id, ct);
    }

    public bool ShouldSkip(StormEventsReportEntity inventory)
    {
        return inventory is { IsDailySummaryComplete: true, IsDailyDetailComplete: true };
    }

    public async Task<StormEventsReportEntity> SourceInventory(DateTime effectiveDate, List<StormEventsReportEntity> inventoryList, CancellationToken ct)
    {
        var inventory = business.GetLatest(effectiveDate, inventoryList);

        if (inventory is null) return await business.DownloadNew(effectiveDate, ct);

        return await business.DownloadUpdate(inventory, ct);
    }
}