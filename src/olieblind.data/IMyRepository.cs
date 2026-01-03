using olieblind.data.Entities;

namespace olieblind.data;

public interface IMyRepository
{
    #region ProductMap

    Task ProductMapCreate(ProductMapEntity entity, CancellationToken ct);

    Task ProductVideoUpdate(ProductVideoEntity entity, CancellationToken ct);

    Task<ProductMapEntity?> ProductMapGet(int id, CancellationToken ct);

    Task<ProductMapEntity> ProductMapGetLatest(CancellationToken ct);

    Task<List<ProductMapEntity>> ProductMapList(CancellationToken ct);

    Task ProductMapUpdate(ProductMapEntity entity, CancellationToken ct);

    #endregion

    #region ProductMapItem

    Task ProductMapItemCreate(ProductMapItemEntity entity, CancellationToken ct);

    Task<List<ProductMapItemEntity>> ProductMapItemList(int productMapId, CancellationToken ct);

    Task ProductMapItemUpdate(ProductMapItemEntity entity, CancellationToken ct);

    #endregion

    #region ProductVideo

    Task ProductVideoCreate(ProductVideoEntity entity, CancellationToken ct);

    Task<ProductVideoEntity?> ProductVideoGet(int id, CancellationToken ct);

    Task<List<ProductVideoEntity>> ProductVideoGetList(CancellationToken ct);

    Task<List<ProductVideoEntity>> ProductVideoGetListMostRecent(CancellationToken ct);

    #endregion

    #region RadarInventory

    Task RadarInventoryAdd(RadarInventoryEntity entity, CancellationToken ct);

    Task<RadarInventoryEntity?> RadarInventoryGet(string id, string effectiveDate, string bucket, CancellationToken ct);

    #endregion

    #region RadarSite

    Task<List<RadarSiteEntity>> RadarSiteGetAll(CancellationToken ct);

    Task RadarSiteCreate(List<RadarSiteEntity> entities, CancellationToken ct);

    #endregion

    #region SpcMesoProduct

    Task SpcMesoProductCreate(SpcMesoProductEntity entity, CancellationToken ct);

    Task<SpcMesoProductEntity?> SpcMesoProductGet(int year, int index, CancellationToken ct);

    Task<SpcMesoProductEntity?> SpcMesoProductGetLatest(int year, CancellationToken ct);

    Task SpcMesoProductUpdate(SpcMesoProductEntity entity, CancellationToken ct);

    #endregion

    #region StormEventsDailyDetail

    Task StormEventsDailyDetailCreate(List<StormEventsDailyDetailEntity> entities, CancellationToken ct);

    Task StormEventsDailyDetailDelete(string dateFk, string sourceFk, CancellationToken ct);

    Task<int> StormEventsDailyDetailCount(string dateFk, string sourceFk, CancellationToken ct);

    #endregion

    #region StormEventsDailySummary

    Task StormEventsDailySummaryCreate(StormEventsDailySummaryEntity entity, CancellationToken ct);

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

    //public async Task<List<StormEventsDailySummaryEntity>> StormEventsDailySummaryListSevereForYear(int year,
    //    CancellationToken ct)
    //{
    //    return await context.StormEventsDailySummary
    //        .Where(w =>
    //            w.Year == year)
    //        .ToListAsync(ct);
    //}

    Task<List<StormEventsDailySummaryEntity>> StormEventsDailySummaryGet(string effectiveDate, int year, CancellationToken ct);

    Task StormEventsDailySummaryUpdate(StormEventsDailySummaryEntity entity, CancellationToken ct);

    #endregion

    #region StormEventsDatabase

    Task StormEventsDatabaseCreate(StormEventsDatabaseEntity entity, CancellationToken ct);

    Task<StormEventsDatabaseEntity?> StormEventsDatabaseGet(int year, string id, CancellationToken ct);

    Task<List<StormEventsDatabaseEntity>> StormEventsDatabaseGetAll(CancellationToken ct);

    Task StormEventsDatabaseInventoryUpdate(StormEventsDatabaseEntity entity, CancellationToken ct);

    #endregion

    #region StormEventsReport

    Task StormEventsReportCreate(StormEventsReportEntity entity, CancellationToken ct);

    Task<List<StormEventsReportEntity>> StormEventsReportsByYear(int year, CancellationToken ct);

    Task StormEventsReportUpdate(StormEventsReportEntity entity, CancellationToken ct);

    #endregion

    #region UserCookieConsentLog

    Task UserCookieConsentLogCreate(UserCookieConsentLogEntity entity, CancellationToken ct);

    #endregion
}
