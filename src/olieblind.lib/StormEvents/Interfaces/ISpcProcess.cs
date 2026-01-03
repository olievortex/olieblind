using olieblind.data.Entities;
using olieblind.lib.StormEvents.Models;

namespace olieblind.lib.StormEvents.Interfaces;

public interface ISpcProcess
{
    Task<(int start, int stop, List<StormEventsReportEntity>)> GetInventoryByYear(int year, CancellationToken ct);

    Task ProcessEvents(List<DailyDetailModel> events, StormEventsReportEntity inventory, CancellationToken ct);

    bool ShouldSkip(StormEventsReportEntity inventory);

    Task<StormEventsReportEntity> SourceInventory(DateTime effectiveDate, List<StormEventsReportEntity> inventoryList, CancellationToken ct);
}