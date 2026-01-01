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

    #region RadarSite

    Task<List<RadarSiteEntity>> RadarSiteGetAll(CancellationToken ct);

    Task RadarSiteCreate(List<RadarSiteEntity> entities, CancellationToken ct);

    #endregion

    #region StormEventsDatabase

    Task<List<StormEventsDatabaseEntity>> StormEventsDatabaseGetAll(CancellationToken ct);

    Task StormEventsDatabaseCreate(StormEventsDatabaseEntity entity, CancellationToken ct);


    //public async Task<StormEventsDatabaseInventoryEntity?> StormEventsDatabaseInventoryGetAsync(int year, string id,
    //    CancellationToken ct)
    //{
    //    return await context.StormEventsDatabaseInventory.SingleOrDefaultAsync(s =>
    //        s.Id == id &&
    //        s.Year == year, ct);
    //}

    //public async Task StormEventsDatabaseInventoryUpdateAsync(StormEventsDatabaseInventoryEntity entity,
    //    CancellationToken ct)
    //{
    //    context.StormEventsDatabaseInventory.Update(entity);
    //    await context.SaveChangesAsync(ct);
    //}

    #endregion

    #region UserCookieConsentLog

    Task UserCookieConsentLogCreate(UserCookieConsentLogEntity entity, CancellationToken ct);

    #endregion
}
