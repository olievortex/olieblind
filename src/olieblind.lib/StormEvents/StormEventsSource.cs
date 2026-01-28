using olieblind.data;
using olieblind.data.Entities;
using olieblind.data.Models;
using olieblind.lib.Mapping;
using olieblind.lib.StormEvents.Interfaces;
using System.Globalization;

namespace olieblind.lib.StormEvents;

public class StormEventsSource(IMyRepository repo) : IStormEventsSource
{
    public DateTime? FromEffectiveDate(string value)
    {
        if (!DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.None,
                out var parsed))
            return null;

        return new DateTime(parsed.Year, parsed.Month, parsed.Day, 0, 0, 0, DateTimeKind.Utc);
    }

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

    public List<SatelliteProductEntity> GetIemSatelliteList()
    {
        var current = new TimeSpan(0, 18, 0, 0);
        var stop = new TimeSpan(0, 24, 0, 0);
        var result = new List<SatelliteProductEntity>();

        while (current < stop)
        {
            var entity = new SatelliteProductEntity
            {
                ScanTime = new DateTime(2010, 1, 1,
                    current.Hours, current.Minutes, 0, DateTimeKind.Utc)
            };
            result.Add(entity);
            current = current.Add(new TimeSpan(0, 15, 0));
        }

        return result;
    }

    public async Task<SpcMesoProductEntity?> GetMeso(int year, int id, CancellationToken ct)
    {
        return await repo.SpcMesoProductGet(year, id, ct);
    }

    public async Task<List<SpcMesoProductEntity>> GetMesoList(string effectiveDate, CancellationToken ct)
    {
        return await repo.SpcMesoProductGetList(effectiveDate, ct);
    }

    public async Task<RadarInventoryEntity?> GetRadarInventory(string radarId, string effectiveDate, string bucketName, CancellationToken ct)
    {
        var result = await repo.RadarInventoryGet(radarId, effectiveDate, bucketName, ct);

        if (result is not null)
            EntityCompression.Decompress(result);

        return result;
    }
}
