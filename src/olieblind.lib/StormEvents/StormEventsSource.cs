using olieblind.data;
using olieblind.data.Entities;
using olieblind.data.Models;
using olieblind.lib.StormEvents.Interfaces;

namespace olieblind.lib.StormEvents;

public class StormEventsSource(IMyRepository repo) : IStormEventsSource
{
    public async Task<List<StormEventsAnnualSummaryModel>> GetAnnualSummaryList(CancellationToken ct)
    {
        return await repo.StormEventsAnnualSummaryList(ct);
    }

    public async Task<StormEventsDailySummaryEntity?> GetDailySummaryByDate(string effectiveDate, int year, CancellationToken ct)
    {
        var summaries = await repo.StormEventsDailySummaryListByDate(effectiveDate, year, ct);
        var topSummary = summaries
            .OrderByDescending(o => o.Timestamp)
            .FirstOrDefault();

        return topSummary;
    }

    public async Task<List<StormEventsDailySummaryEntity>> GetDailySummaryList(int year, CancellationToken ct)
    {
        var summaries = await repo.StormEventsDailySummaryListByYear(year, ct);

        var topRowIds = summaries.GroupBy(g => g.Id)
            .Select(s => new
            {
                Id = s.Key,
                Timestamp = s.Max(m => m.Timestamp)
            })
            .ToList();
        var topRows = summaries.Join(topRowIds,
                l => new
                {
                    l.Id,
                    l.Timestamp
                },
                r => new
                {
                    r.Id,
                    r.Timestamp
                },
                (l, _) => l)
            .ToList();

        return topRows;
    }
}
