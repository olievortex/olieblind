using Azure.Storage.Blobs;
using olieblind.data.Entities;
using olieblind.lib.StormEvents.Models;

namespace olieblind.lib.StormEvents.Interfaces;

public interface IDatabaseProcess
{
    Task<List<StormEventsDailySummaryEntity>> DeactivateOldSummaries(string id, int year, string sourceFk,
        CancellationToken ct);

    List<DailySummaryModel> GetAggregate(List<DailyDetailModel> models);

    Task<List<DailyDetailModel>> Load(
        BlobContainerClient blobClient, StormEventsDatabaseEntity eventsDatabase, CancellationToken ct);

    Task SourceDatabases(BlobContainerClient blobClient, CancellationToken ct);
}