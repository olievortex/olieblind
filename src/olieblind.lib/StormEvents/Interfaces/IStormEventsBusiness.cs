using olieblind.lib.StormEvents.Models;

namespace olieblind.lib.StormEvents.Interfaces;

public interface IStormEventsBusiness
{
    Task<AnnualOverviewModel> GetAnnualOverview(int year, CancellationToken ct);

    Task<DailyOverviewModel?> GetDailyOverview(string effectiveDate, string sourceFk, CancellationToken ct);

    Task<DailyDetailIdentifierModel?> GetDailyDetailIdentifierByDate(string effectiveDate, CancellationToken ct);
}
