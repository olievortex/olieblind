using Azure.Storage.Blobs;
using olieblind.data.Entities;

namespace olieblind.lib.StormEvents.Interfaces;

public interface IDatabaseProcess
{
    Task<List<StormEventsDailySummaryEntity>> DeactivateOldSummaries(string effectiveDate, int year, string sourceFk, CancellationToken ct);

    Task SourceDatabases(BlobContainerClient blobClient, CancellationToken ct);
}