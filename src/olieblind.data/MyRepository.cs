using Microsoft.EntityFrameworkCore;
using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.data.Models;
using System.Diagnostics.CodeAnalysis;

namespace olieblind.data;

[ExcludeFromCodeCoverage]
public class MyRepository(MyContext context) : IMyRepository
{
    private const int VisibleSat = 2;

    #region ProductMap

    public async Task ProductMapCreate(ProductMapEntity entity, CancellationToken ct)
    {
        await context.ProductMaps.AddAsync(entity, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<ProductMapEntity?> ProductMapGet(int id, CancellationToken ct)
    {
        return await context.ProductMaps
            .AsNoTracking()
            .Where(w => w.Id == id)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<ProductMapEntity> ProductMapGetLatest(CancellationToken ct)
    {
        return await context.ProductMaps
            .AsNoTracking()
            .Where(w => w.IsActive)
            .OrderByDescending(o => o.Timestamp)
            .FirstAsync(ct);
    }

    public async Task<List<ProductMapEntity>> ProductMapList(CancellationToken ct)
    {
        return await context.ProductMaps
            .AsNoTracking()
            .Where(w => w.IsActive)
            .OrderByDescending(o => o.Timestamp)
            .ToListAsync(ct);
    }

    public async Task ProductMapUpdate(ProductMapEntity entity, CancellationToken ct)
    {
        context.ProductMaps.Update(entity);
        await context.SaveChangesAsync(ct);
    }

    #endregion

    #region ProductMapItem

    public async Task ProductMapItemCreate(ProductMapItemEntity entity, CancellationToken ct)
    {
        await context.ProductMapItems.AddAsync(entity, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<List<ProductMapItemEntity>> ProductMapItemList(int productMapId, CancellationToken ct)
    {
        return await context.ProductMapItems
            .AsNoTracking()
            .Where(w => w.ProductMapId == productMapId && w.IsActive)
            .OrderBy(o => o.ParameterId)
            .ThenBy(o => o.GeographyId)
            .ToListAsync(ct);
    }

    public async Task ProductMapItemUpdate(ProductMapItemEntity entity, CancellationToken ct)
    {
        context.ProductMapItems.Update(entity);
        await context.SaveChangesAsync(ct);
    }

    #endregion

    #region ProductVideo

    public async Task ProductVideoCreate(ProductVideoEntity entity, CancellationToken ct)
    {
        await context.ProductVideos.AddAsync(entity, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task ProductVideoUpdate(ProductVideoEntity entity, CancellationToken ct)
    {
        context.ProductVideos.Update(entity);
        await context.SaveChangesAsync(ct);
    }

    public async Task<ProductVideoEntity?> ProductVideoGet(int id, CancellationToken ct)
    {
        return await context.ProductVideos
            .AsNoTracking()
            .SingleOrDefaultAsync(s => s.Id == id, ct);
    }

    public async Task<List<ProductVideoEntity>> ProductVideoGetList(CancellationToken ct)
    {
        return await context.ProductVideos
            .AsNoTracking()
            .Where(w => w.IsActive)
            .OrderByDescending(o => o.Timestamp)
            .ToListAsync(ct);
    }

    public async Task<List<ProductVideoEntity>> ProductVideoGetListMostRecent(CancellationToken ct)
    {
        var keys = await context.ProductVideos
            .AsNoTracking()
            .Where(w => w.IsActive)
            .GroupBy(g => g.Category)
            .Select(g => g.Max(m => m.Id))
            .ToListAsync(ct);

        return await context.ProductVideos
            .Where(w => keys.Contains(w.Id))
            .ToListAsync(ct);
    }

    #endregion

    #region RadarInventory

    public async Task RadarInventoryAdd(RadarInventoryEntity entity, CancellationToken ct)
    {
        await context.RadarInventories.AddAsync(entity, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<RadarInventoryEntity?> RadarInventoryGet(string id, string effectiveDate, string bucket, CancellationToken ct)
    {
        return await context.RadarInventories.AsNoTracking().SingleOrDefaultAsync(s =>
            s.Id == id &&
            s.EffectiveDate == effectiveDate &&
            s.BucketName == bucket, ct);
    }

    #endregion

    #region RadarSite

    public async Task<List<RadarSiteEntity>> RadarSiteGetAll(CancellationToken ct)
    {
        return await context.RadarSites.AsNoTracking().ToListAsync(ct);
    }

    public async Task RadarSiteCreate(List<RadarSiteEntity> entities, CancellationToken ct)
    {
        await context.RadarSites.AddRangeAsync(entities, ct);
        await context.SaveChangesAsync(ct);
    }

    #endregion

    #region SatelliteAwsInventory

    public async Task SatelliteAwsInventoryCreate(SatelliteAwsInventoryEntity entity, CancellationToken ct)
    {
        await context.SatelliteAwsInventories.AddAsync(entity, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<List<SatelliteAwsInventoryEntity>> SatelliteAwsInventoryListByYear(int year, int channel,
        DayPartsEnum dayPart, CancellationToken ct)
    {
        return await context.SatelliteAwsInventories
            .Where(w =>
                w.EffectiveDate.StartsWith($"{year}-") &&
                w.Channel == channel &&
                w.DayPart == dayPart)
            .ToListAsync(ct);
    }

    #endregion

    #region SatelliteAwsProduct

    public async Task SatelliteAwsProductCreate(List<SatelliteAwsProductEntity> entity, CancellationToken ct)
    {
        context.SatelliteAwsProducts.AddRange(entity);
        await context.SaveChangesAsync(ct);
    }

    public async Task<SatelliteAwsProductEntity?> SatelliteAwsProductGetLastPoster(string effectiveDate, CancellationToken ct)
    {
        return await context.SatelliteAwsProducts
            .Where(w => w.EffectiveDate == effectiveDate)
            .OrderByDescending(o => o.ScanTime)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<SatelliteAwsProductEntity?> SatelliteAwsProductGetPoster(string effectiveDate, DateTime eventTime, CancellationToken ct)
    {
        return await context.SatelliteAwsProducts
            .Where(w => w.EffectiveDate == effectiveDate &&
                        w.ScanTime >= eventTime)
            .OrderBy(o => o.ScanTime)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<List<SatelliteAwsProductEntity>> SatelliteAwsProductGetList(string effectiveDate, CancellationToken ct)
    {
        return await context.SatelliteAwsProducts
            .AsNoTracking()
            .Where(w =>
                w.EffectiveDate == effectiveDate &&
                w.Channel == VisibleSat &&
                w.DayPart == DayPartsEnum.Afternoon)
            .OrderBy(o => o.ScanTime)
            .ToListAsync(ct);
    }

    public async Task<List<SatelliteAwsProductEntity>> SatelliteAwsProductListNoPoster(CancellationToken ct)
    {
        return await context.SatelliteAwsProducts
            .Where(w =>
                w.Path1080 != null &&
                w.PathPoster == null)
            .OrderBy(o => o.ScanTime)
            .ToListAsync(ct);
    }

    public async Task SatelliteAwsProductUpdate(SatelliteAwsProductEntity entity, CancellationToken ct)
    {
        context.SatelliteAwsProducts.Update(entity);
        await context.SaveChangesAsync(ct);
    }

    #endregion

    #region SpcMesoProduct

    public async Task SpcMesoProductCreate(SpcMesoProductEntity entity, CancellationToken ct)
    {
        await context.SpcMesoProducts.AddAsync(entity, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<SpcMesoProductEntity?> SpcMesoProductGet(int year, int index, CancellationToken ct)
    {
        return await context.SpcMesoProducts
            .AsNoTracking()
            .Where(w =>
                w.EffectiveTime.Year == year &&
                w.Id == index)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<int> SpcMesoProductGetCount(string effectiveDate, CancellationToken ct)
    {
        return await context.SpcMesoProducts.CountAsync(c =>
            c.EffectiveDate == effectiveDate, ct);
    }

    public async Task<SpcMesoProductEntity?> SpcMesoProductGetLatest(int year, CancellationToken ct)
    {
        return await context.SpcMesoProducts
            .AsNoTracking()
            .Where(w => w.EffectiveTime.Year == year)
            .OrderByDescending(o => o.Id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<List<SpcMesoProductEntity>> SpcMesoProductGetList(string effectiveDate, CancellationToken ct)
    {
        return await context.SpcMesoProducts
            .AsNoTracking()
            .Where(c => c.EffectiveDate == effectiveDate)
            .ToListAsync(ct);
    }

    public async Task SpcMesoProductUpdate(SpcMesoProductEntity entity, CancellationToken ct)
    {
        context.SpcMesoProducts.Update(entity);
        await context.SaveChangesAsync(ct);
    }

    #endregion

    #region StormEventsAnnualSummary

    public async Task<List<StormEventsAnnualSummaryModel>> StormEventsAnnualSummaryList(CancellationToken ct)
    {
        return [.. context.StormEventsDailySummaries
            .Where(w => w.IsCurrent)
            .GroupBy(g => g.Year)
            .Select(s => new StormEventsAnnualSummaryModel
            {
                Year = s.Key,
                SevereDays = s.Count(),
                HailReports = s.Sum(u => u.Hail),
                WindReports = s.Sum(u => u.Wind),
                ExtremeTornadoes = s.Sum(u => u.F5 + u.F4),
                StrongTornadoes = s.Sum(u => u.F3 + u.F2),
                OtherTornadoes = s.Sum(u => u.F1)
            })
            .OrderByDescending(d => d.Year)];
    }

    #endregion

    #region StormEventsDailyDetail

    public async Task<int> StormEventsDailyDetailCount(string dateFk, string sourceFk, CancellationToken ct)
    {
        return await context.StormEventsDailyDetails
            .CountAsync(w =>
                w.DateFk == dateFk &&
                w.SourceFk == sourceFk, ct);
    }

    public async Task StormEventsDailyDetailCreate(List<StormEventsDailyDetailEntity> entities, CancellationToken ct)
    {
        await context.StormEventsDailyDetails.AddRangeAsync(entities, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task StormEventsDailyDetailDelete(string dateFk, string sourceFk, CancellationToken ct)
    {
        var items = await context.StormEventsDailyDetails
            .Where(w =>
                w.DateFk == dateFk &&
                w.SourceFk == sourceFk)
            .ToListAsync(ct);

        foreach (var item in items)
        {
            context.StormEventsDailyDetails.Remove(item);
        }

        await context.SaveChangesAsync(ct);
    }

    public async Task<List<StormEventsDailyDetailEntity>> StormEventsDailyDetailList(string effectiveDate, string sourceFk, CancellationToken ct)
    {
        return await context.StormEventsDailyDetails
            .AsNoTracking()
            .Where(w =>
                w.DateFk == effectiveDate &&
                w.SourceFk == sourceFk)
            .ToListAsync(ct);
    }

    #endregion

    #region StormEventsDailySummary

    public async Task StormEventsDailySummaryCreate(StormEventsDailySummaryEntity entity, CancellationToken ct)
    {
        await context.StormEventsDailySummaries.AddAsync(entity, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<List<StormEventsDailySummaryEntity>> StormEventsDailySummaryListByDate(string effectiveDate, int year, CancellationToken ct)
    {
        return await context.StormEventsDailySummaries
            .AsNoTracking()
            .Where(w =>
                w.Year == year &&
                w.Id == effectiveDate &&
                w.IsCurrent)
            .ToListAsync(ct);
    }

    public async Task<List<StormEventsDailySummaryEntity>> StormEventsDailySummaryListByYear(int year, CancellationToken ct)
    {
        return await context.StormEventsDailySummaries
            .Where(w => w.Year == year)
            .ToListAsync(ct);
    }

    public async Task<List<StormEventsDailySummaryEntity>> StormEventsDailySummaryListMissingPostersForYear(int year, CancellationToken ct)
    {
        return await context.StormEventsDailySummaries
            .Where(w =>
                w.Year == year &&
                w.HeadlineEventTime != null &&
                (w.SatellitePath1080 == null || w.SatellitePathPoster == null))
            .ToListAsync(ct);
    }

    public async Task<List<StormEventsDailySummaryEntity>> StormEventsDailySummaryGet(string effectiveDate, int year, CancellationToken ct)
    {
        return await context.StormEventsDailySummaries
            .AsNoTracking()
            .Where(w =>
                w.Id == effectiveDate &&
                w.Year == year)
            .ToListAsync(ct);
    }

    public async Task<StormEventsDailySummaryEntity?> StormEventsDailySummaryGet(string effectiveDate, int year, string sourceFk, CancellationToken ct)
    {
        return await context.StormEventsDailySummaries
            .AsNoTracking()
            .SingleOrDefaultAsync(s =>
                s.Id == effectiveDate &&
                s.SourceFk == sourceFk &&
                s.Year == year, ct);
    }

    public async Task StormEventsDailySummaryUpdate(StormEventsDailySummaryEntity entity, CancellationToken ct)
    {
        context.StormEventsDailySummaries.Update(entity);
        await context.SaveChangesAsync(ct);
    }

    #endregion

    #region StormEventsDatabase

    public async Task StormEventsDatabaseCreate(StormEventsDatabaseEntity entity, CancellationToken ct)
    {
        await context.StormEventsDatabases.AddAsync(entity, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<StormEventsDatabaseEntity?> StormEventsDatabaseGet(int year, string id, CancellationToken ct)
    {
        return await context.StormEventsDatabases.AsNoTracking().SingleOrDefaultAsync(s =>
            s.Id == id &&
            s.Year == year, ct);
    }

    public async Task<List<StormEventsDatabaseEntity>> StormEventsDatabaseGetAll(CancellationToken ct)
    {
        return await context.StormEventsDatabases.AsNoTracking().ToListAsync(ct);
    }

    public async Task StormEventsDatabaseInventoryUpdate(StormEventsDatabaseEntity entity, CancellationToken ct)
    {
        context.StormEventsDatabases.Update(entity);
        await context.SaveChangesAsync(ct);
    }

    #endregion

    #region StormEventsReport

    public async Task StormEventsReportCreate(StormEventsReportEntity entity, CancellationToken ct)
    {
        await context.StormEventsReports.AddAsync(entity, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<List<StormEventsReportEntity>> StormEventsReportsByYear(int year, CancellationToken ct)
    {
        var yearValue = $"{year}-";

        return await context.StormEventsReports
            .AsNoTracking()
            .Where(w => w.EffectiveDate.StartsWith(yearValue))
            .ToListAsync(ct);
    }

    public async Task StormEventsReportUpdate(StormEventsReportEntity entity, CancellationToken ct)
    {
        context.StormEventsReports.Update(entity);
        await context.SaveChangesAsync(ct);
    }

    #endregion

    #region UserCookieConsentLog

    public async Task UserCookieConsentLogCreate(UserCookieConsentLogEntity entity, CancellationToken ct)
    {
        await context.UserCookieConsentLogs.AddAsync(entity, ct);
        await context.SaveChangesAsync(ct);
    }

    #endregion

    #region UserSatelliteAdHocLog

    public async Task UserSatelliteAdHocLogCreate(UserSatelliteAdHocLogEntity entity, CancellationToken ct)
    {
        await context.UserSatelliteAdHocLogs.AddAsync(entity, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<Dictionary<string, int>> UserSatelliteAdHocLogUserStatistics(int lookbackHours, CancellationToken ct)
    {
        return await context.UserSatelliteAdHocLogs
            .AsNoTracking()
            .Where(w => w.Timestamp >= DateTime.UtcNow.AddHours(-lookbackHours) && !w.IsFree)
            .GroupBy(g => g.Id)
            .Select(s => new
            {
                UserId = s.Key,
                Count = s.Count()
            })
            .ToDictionaryAsync(k => k.UserId, v => v.Count, ct);
    }

    #endregion
}
