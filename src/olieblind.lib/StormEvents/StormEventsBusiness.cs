using olieblind.lib.StormEvents.Interfaces;
using olieblind.lib.StormEvents.Models;

namespace olieblind.lib.StormEvents;

public class StormEventsBusiness(IStormEventsSource source) : IStormEventsBusiness
{
    public async Task<AnnualOverviewModel> GetAnnualOverview(int year, CancellationToken ct)
    {
        var events = await source.GetDailySummaryList(year, ct);

        var strongTornadoes = events
            .Where(w => w.F3 > 0 || w.F2 > 0)
            .OrderBy(o => o.Id)
            .Select(s => new AnnualOverviewModel.AnnualOverviewItem(s))
            .ToList();

        var extremeTornadoes = events
            .Where(w => w.F5 > 0 || w.F4 > 0)
            .OrderBy(o => o.Id)
            .Select(s => new AnnualOverviewModel.AnnualOverviewItem(s))
            .ToList();

        var hailTop10 = events
            .Where(w => w.Hail > 1)
            .OrderByDescending(desc => desc.Hail)
            .Take(10)
            .Select(s => new AnnualOverviewModel.AnnualOverviewItem(s))
            .ToList();

        var windTop10 = events
            .Where(w => w.Wind > 1)
            .OrderByDescending(desc => desc.Wind)
            .Take(10)
            .Select(s => new AnnualOverviewModel.AnnualOverviewItem(s))
            .ToList();

        var tornadoTop10 = events
            .Select(s => new AnnualOverviewModel.AnnualOverviewItem(s))
            .Where(w => w.TornadoCount > 1)
            .OrderByDescending(desc => desc.TornadoCount)
            .Take(10)
            .ToList();

        var recent10 = events
            .Select(s => new AnnualOverviewModel.AnnualOverviewItem(s))
            .OrderByDescending(desc => desc.Id)
            .Take(10)
            .ToList();

        var result = new AnnualOverviewModel
        {
            HailTop10 = hailTop10,
            TornadoTop10 = tornadoTop10,
            WindTop10 = windTop10,
            Recent10 = recent10,
            ExtremeTornadoes = extremeTornadoes,
            StrongTornadoes = strongTornadoes
        };

        return result;
    }

    public async Task<DailyDetailIdentifierModel?> GetDailyDetailIdentifierByDate(string effectiveDate, CancellationToken ct)
    {
        var year = int.Parse(effectiveDate[..4]);
        var summary = await source.GetDailySummaryByDate(effectiveDate, year, ct);
        if (summary is null) return null;

        return new DailyDetailIdentifierModel
        {
            SourceFk = summary.SourceFk,
            EffectiveDate = summary.Id,
            Year = summary.Year
        };
    }
}
