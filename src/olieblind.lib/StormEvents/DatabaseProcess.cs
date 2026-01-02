using Azure.Storage.Blobs;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.StormEvents.Interfaces;

namespace olieblind.lib.StormEvents;

public class DatabaseProcess(IDatabaseBusiness business, IMyRepository repo) : IDatabaseProcess
{
    public async Task<List<StormEventsDailySummaryEntity>> DeactivateOldSummaries(string effectiveDate, int year, string update, CancellationToken ct)
    {
        var summaries = await repo.StormEventsDailySummaryGet(effectiveDate, year, ct);

        foreach (var summary in summaries)
        {
            if (summary.SourceFk != update && summary.IsCurrent)
            {
                summary.IsCurrent = false;
                summary.Timestamp = DateTime.UtcNow;
                await repo.StormEventsDailySummaryUpdate(summary, ct);
            }
        }

        return summaries;
    }

    public async Task SourceDatabases(BlobContainerClient blobClient, CancellationToken ct)
    {
        var eventsList = await business.DatabaseList(ct);
        await business.DatabaseDownload(blobClient, eventsList, ct);
    }
}