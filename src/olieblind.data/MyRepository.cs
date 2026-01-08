using Microsoft.EntityFrameworkCore;
using olieblind.data.Entities;
using olieblind.data.Enums;
using System.Diagnostics.CodeAnalysis;

namespace olieblind.data;

[ExcludeFromCodeCoverage]
public class MyRepository(MyContext context) : IMyRepository
{
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

    public async Task<SpcMesoProductEntity?> SpcMesoProductGetLatest(int year, CancellationToken ct)
    {
        return await context.SpcMesoProducts
            .AsNoTracking()
            .Where(w => w.EffectiveTime.Year == year)
            .OrderByDescending(o => o.Id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task SpcMesoProductUpdate(SpcMesoProductEntity entity, CancellationToken ct)
    {
        context.SpcMesoProducts.Update(entity);
        await context.SaveChangesAsync(ct);
    }

    #endregion

    #region StormEventsDailyDetail

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

    public async Task<int> StormEventsDailyDetailCount(string dateFk, string sourceFk, CancellationToken ct)
    {
        return await context.StormEventsDailyDetails
            .CountAsync(w =>
                w.DateFk == dateFk &&
                w.SourceFk == sourceFk, ct);
    }

    #endregion

    #region StormEventsDailySummary

    public async Task StormEventsDailySummaryCreate(StormEventsDailySummaryEntity entity, CancellationToken ct)
    {
        await context.StormEventsDailySummaries.AddAsync(entity, ct);
        await context.SaveChangesAsync(ct);
    }

    //public async Task<List<StormEventsDailySummaryEntity>> StormEventsDailySummaryListMissingPostersForYear(int year,
    //    CancellationToken ct)
    //{
    //    return await context.StormEventsDailySummary
    //        .Where(w =>
    //            w.Year == year &&
    //            w.HeadlineEventTime != null &&
    //            // ReSharper disable once EntityFramework.UnsupportedServerSideFunctionCall
    //            (EF.Functions.CoalesceUndefined(w.SatellitePath1080, null) == null ||
    //             // ReSharper disable once EntityFramework.UnsupportedServerSideFunctionCall
    //             EF.Functions.CoalesceUndefined(w.SatellitePathPoster, null) == null))
    //        .ToListAsync(ct);
    //}

    public async Task<List<StormEventsDailySummaryEntity>> StormEventsDailySummaryListByYear(int year, CancellationToken ct)
    {
        return await context.StormEventsDailySummaries
            .Where(w => w.Year == year)
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
}
