using olieblind.data.Models;

namespace olieblind.lib.StormEvents.Interfaces;

public interface IStormEventsSource
{
    Task<List<StormEventsAnnualSummaryModel>> GetAnnualSummaryList(CancellationToken ct);
}
