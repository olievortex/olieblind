using Microsoft.EntityFrameworkCore;
using olieblind.data.Entities;
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

    #region StormEventsDatabase

    public async Task<List<StormEventsDatabaseEntity>> StormEventsDatabaseGetAll(CancellationToken ct)
    {
        return await context.StormEventsDatabases.AsNoTracking().ToListAsync(ct);
    }

    public async Task StormEventsDatabaseCreate(StormEventsDatabaseEntity entity, CancellationToken ct)
    {
        await context.StormEventsDatabases.AddAsync(entity, ct);
        await context.SaveChangesAsync(ct);
    }

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

    public async Task UserCookieConsentLogCreate(UserCookieConsentLogEntity entity, CancellationToken ct)
    {
        await context.UserCookieConsentLogs.AddAsync(entity, ct);
        await context.SaveChangesAsync(ct);
    }

    #endregion
}
