using Azure.Storage.Blobs;
using olieblind.data.Entities;
using olieblind.lib.StormEvents.Models;

namespace olieblind.lib.StormEvents.Interfaces;

public interface IDatabaseBusiness
{
    #region Detail

    Task AddDailyDetailToCosmos(List<DailyDetailModel> models, string sourceFk, CancellationToken ct);

    Task CompareDetailCount(string dateFk, string sourceFk, int count, CancellationToken ct);

    #endregion

    #region Summary

    Task ActivateSummary(StormEventsDailySummaryEntity entity, CancellationToken ct);

    Task AddDailySummaryToCosmos(DailySummaryModel model, string sourceFk, CancellationToken ct);

    #endregion

    #region Database

    Task DatabaseDownload(BlobContainerClient client, List<DatabaseFileModel> model, CancellationToken ct);

    Task<List<DatabaseFileModel>> DatabaseList(CancellationToken ct);

    Task<List<DailyDetailModel>> DatabaseLoad(BlobContainerClient blobClient, StormEventsDatabaseEntity eventsDatabase, CancellationToken ct);

    Task DatabaseUpdateActive(StormEventsDatabaseEntity entity, CancellationToken ct);

    Task DatabaseUpdateRowCount(StormEventsDatabaseEntity entity, int rowCount, CancellationToken ct);

    #endregion
}