using olieblind.data.Entities;

namespace olieblind.data;

public interface IMyRepository
{
    #region ProductMap

    Task ProductMapCreate(ProductMap entity, CancellationToken ct);

    Task ProductVideoUpdate(ProductVideoEntity entity, CancellationToken ct);

    Task<ProductMap?> ProductMapGet(int id, CancellationToken ct);

    Task<ProductMap> ProductMapGetLatest(CancellationToken ct);

    Task<List<ProductMap>> ProductMapList(CancellationToken ct);

    Task ProductMapUpdate(ProductMap entity, CancellationToken ct);

    #endregion

    #region ProductMapItem

    Task ProductMapItemCreate(ProductMapItem entity, CancellationToken ct);

    Task<List<ProductMapItem>> ProductMapItemList(int productMapId, CancellationToken ct);

    Task ProductMapItemUpdate(ProductMapItem entity, CancellationToken ct);

    #endregion

    #region ProductVideo

    Task ProductVideoCreate(ProductVideoEntity entity, CancellationToken ct);

    Task<ProductVideoEntity?> ProductVideoGet(int id, CancellationToken ct);

    Task<List<ProductVideoEntity>> ProductVideoGetList(CancellationToken ct);

    Task<List<ProductVideoEntity>> ProductVideoGetListMostRecent(CancellationToken ct);

    #endregion

    #region UserCookieConsentLog

    Task UserCookieConsentLogCreate(UserCookieConsentLogEntity entity, CancellationToken ct);

    #endregion
}
