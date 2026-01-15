using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.data.Models;

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

    #region SatelliteAwsInventory

    Task SatelliteAwsInventoryCreate(SatelliteAwsInventoryEntity entity, CancellationToken ct);

    Task<List<SatelliteAwsInventoryEntity>> SatelliteAwsInventoryListByYear(int year, int channel, DayPartsEnum dayPart, CancellationToken ct);

    #endregion

    #region SatelliteAwsProduct

    Task SatelliteAwsProductCreate(List<SatelliteAwsProductEntity> entity, CancellationToken ct);

    Task<SatelliteAwsProductEntity?> SatelliteAwsProductGetLastPoster(string effectiveDate, CancellationToken ct);

    Task<SatelliteAwsProductEntity?> SatelliteAwsProductGetPoster(string effectiveDate, DateTime eventTime, CancellationToken ct);

    Task<List<SatelliteAwsProductEntity>> SatelliteAwsProductListNoPoster(CancellationToken ct);

    Task SatelliteAwsProductUpdate(SatelliteAwsProductEntity entity, CancellationToken ct);

    #endregion

    #region SpcMesoProduct

    Task SpcMesoProductCreate(SpcMesoProductEntity entity, CancellationToken ct);

    Task<SpcMesoProductEntity?> SpcMesoProductGet(int year, int index, CancellationToken ct);

    Task<int> SpcMesoProductGetCount(string effectiveDate, CancellationToken ct);

    Task<SpcMesoProductEntity?> SpcMesoProductGetLatest(int year, CancellationToken ct);

    Task<List<SpcMesoProductEntity>> SpcMesoProductGetList(string effectiveDate, CancellationToken ct);

    Task SpcMesoProductUpdate(SpcMesoProductEntity entity, CancellationToken ct);

    #endregion

    #region StormEventsAnnualSummary

    Task<List<StormEventsAnnualSummaryModel>> StormEventsAnnualSummaryList(CancellationToken ct);

    #endregion

    #region StormEventsDailyDetail

    Task<int> StormEventsDailyDetailCount(string dateFk, string sourceFk, CancellationToken ct);

    Task StormEventsDailyDetailCreate(List<StormEventsDailyDetailEntity> entities, CancellationToken ct);

    Task StormEventsDailyDetailDelete(string dateFk, string sourceFk, CancellationToken ct);

    Task<List<StormEventsDailyDetailEntity>> StormEventsDailyDetailList(string effectiveDate, string sourceFk, CancellationToken ct);

    #endregion

    #region StormEventsDailySummary

    Task StormEventsDailySummaryCreate(StormEventsDailySummaryEntity entity, CancellationToken ct);

    Task<List<StormEventsDailySummaryEntity>> StormEventsDailySummaryListByDate(string effectiveDate, int year, CancellationToken ct);

    Task<List<StormEventsDailySummaryEntity>> StormEventsDailySummaryListByYear(int year, CancellationToken ct);

    Task<List<StormEventsDailySummaryEntity>> StormEventsDailySummaryListMissingPostersForYear(int year, CancellationToken ct);

    Task<List<StormEventsDailySummaryEntity>> StormEventsDailySummaryGet(string effectiveDate, int year, CancellationToken ct);

    Task<StormEventsDailySummaryEntity?> StormEventsDailySummaryGet(string effectiveDate, int year, string sourceFk, CancellationToken ct);

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
