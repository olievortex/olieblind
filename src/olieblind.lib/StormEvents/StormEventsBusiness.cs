using olieblind.data;
using olieblind.lib.StormEvents.Interfaces;
using olieblind.lib.StormEvents.Models;

namespace olieblind.lib.StormEvents;

public class StormEventsBusiness(IStormEventsSource source, IMyRepository repo) : IStormEventsBusiness
{
    private const int Goes16 = 2018;
    private const string EimAttribution = "Iowa Environmental Mesonet of Iowa State University";
    private const string AwsAttribution = "NOAA Open Data Dissemination Program";
    private const string IowaMesonetBucket = "IEM";

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

    public async Task<DailyOverviewModel?> GetDailyOverview(string effectiveDate, string sourceFk, CancellationToken ct)
    {
        var year = int.Parse(effectiveDate[..4]);

        var dateValue = source.FromEffectiveDate(effectiveDate);
        if (dateValue is null) return null;

        var summary = await repo.StormEventsDailySummaryGet(effectiveDate, year, sourceFk, ct);
        if (summary is null) return null;

        var events = await repo.StormEventsDailyDetailList(effectiveDate, sourceFk, ct);
        var tornadoes = events
            .Where(w => w.EventType == "Tornado")
            .OrderBy(o => o.DateFk)
            .ThenBy(o => o.Timestamp)
            .ToList();
        var hails = events
            .Where(w => w.EventType == "Hail")
            .OrderBy(o => o.DateFk)
            .ThenBy(o => o.Timestamp)
            .ToList();
        var winds = events
            .Where(w => w.EventType == "Thunderstorm Wind")
            .OrderBy(o => o.DateFk)
            .ThenBy(o => o.Timestamp)
            .ToList();
        var mesos = await repo.SpcMesoProductGetCount(effectiveDate, ct);

        var result = new DailyOverviewModel
        {
            Events = events,
            Tornadoes = tornadoes,
            SatellitePosterPath = summary.SatellitePathPoster ?? summary.SatellitePath1080,
            Satellite1080Path = summary.SatellitePath1080 ?? summary.SatellitePathPoster,
            SatelliteDateTime = summary.HeadlineEventTime,
            SatelliteAttribution = dateValue.Value.Year < Goes16 ? EimAttribution : AwsAttribution,
            Winds = winds,
            Hails = hails,
            MesoCount = mesos
        };

        return result;
    }

    public async Task<SatelliteListModel> GetSatelliteList(string effectiveDate, CancellationToken ct)
    {
        var satList = await repo.SatelliteProductGetList(effectiveDate, ct);
        var awsList = satList.Where(w => w.BucketName != IowaMesonetBucket).ToList();
        var iemList = satList.Where(w => w.BucketName == IowaMesonetBucket).ToList();

        if (iemList.Count == 0) iemList = source.GetIemSatelliteList();

        return new SatelliteListModel
        {
            IemList = iemList,
            AwsList = awsList
        };
    }
}
