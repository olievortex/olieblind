using olieblind.data;
using olieblind.data.Models;
using olieblind.lib.StormEvents.Interfaces;

namespace olieblind.lib.StormEvents;

public class StormEventsSource(IMyRepository repo) : IStormEventsSource
{
    public async Task<List<StormEventsAnnualSummaryModel>> GetAnnualSummaryList(CancellationToken ct)
    {
        return await repo.StormEventsAnnualSummaryList(ct);
    }
}
