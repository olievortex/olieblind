using Azure.Storage.Blobs;
using olieblind.data.Entities;
using olieblind.lib.StormEvents.Models;

namespace olieblind.lib.StormEvents.Interfaces;

public interface IDatabaseProcess
{
    Task<List<StormEventsDailySummaryEntity>> DeactivateOldSummariesAsync(string id, int year, string sourceFk,
        CancellationToken ct);

    List<DailySummaryModel> GetAggregate(List<DailyDetailModel> models);

    Task<List<DailyDetailModel>> LoadAsync(
        BlobContainerClient blobClient, StormEventsDatabaseEntity eventsDatabase, CancellationToken ct);

    Task SourceDatabasesAsync(BlobContainerClient blobClient, CancellationToken ct);
}