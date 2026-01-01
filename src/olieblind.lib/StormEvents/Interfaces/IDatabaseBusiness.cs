using Azure.Storage.Blobs;
using olieblind.data.Entities;
using olieblind.lib.StormEvents.Models;

namespace olieblind.lib.StormEvents.Interfaces;

public interface IDatabaseBusiness
{
    #region Detail

    Task AddDailyDetailToCosmosAsync(List<DailyDetailModel> models, string sourceFk, CancellationToken ct);

    Task CompareDetailCountAsync(string dateFk, string sourceFk, int count, CancellationToken ct);

    Task DeleteDetailAsync(string dateFk, string sourceFk, CancellationToken ct);

    #endregion

    #region Summary

    Task ActivateSummaryAsync(StormEventsDailySummaryEntity entity, CancellationToken ct);

    Task AddDailySummaryToCosmosAsync(DailySummaryModel model, string sourceFk, CancellationToken ct);

    Task DeactivateSummaryAsync(StormEventsDailySummaryEntity entity, CancellationToken ct);

    Task<List<StormEventsDailySummaryEntity>> GetSummariesForDayAsync(string id, int year,
        CancellationToken ct);

    #endregion

    #region Database

    Task DatabaseDownloadAsync(BlobContainerClient client, List<DatabaseFileModel> model, CancellationToken ct);

    Task<StormEventsDatabaseEntity?> DatabaseGetInventoryAsync(int year, string id, CancellationToken ct);

    Task<List<DatabaseFileModel>> DatabaseListAsync(CancellationToken ct);

    Task<List<DailyDetailModel>> DatabaseLoadAsync(
        BlobContainerClient blobClient, StormEventsDatabaseEntity eventsDatabase, CancellationToken ct);

    Task DatabaseUpdateActiveAsync(StormEventsDatabaseEntity entity, CancellationToken ct);

    Task DatabaseUpdateRowCountAsync(StormEventsDatabaseEntity entity, int rowCount, CancellationToken ct);

    #endregion
}