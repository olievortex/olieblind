using Azure.Storage.Blobs;
using olieblind.data.Entities;
using olieblind.lib.StormEvents.Interfaces;
using olieblind.lib.StormEvents.Models;

namespace olieblind.lib.StormEvents;

public class DatabaseProcess(IDatabaseBusiness business) : IDatabaseProcess
{
    //public async Task<List<StormEventsDailySummaryEntity>> DeactivateOldSummariesAsync(string id, int year,
    //    string sourceFk, CancellationToken ct)
    //{
    //    var summaries = await business.GetSummariesForDayAsync(id, year, ct);

    //    var deactivate = summaries.Where(w =>
    //            w.SourceFk != sourceFk &&
    //            w.IsCurrent)
    //        .ToList();

    //    foreach (var item in deactivate) await business.DeactivateSummaryAsync(item, ct);

    //    return summaries;
    //}

    //public List<DailySummaryModel> GetAggregate(List<DailyDetailModel> models)
    //{
    //    return DailySummaryBusiness.AggregateByDate(models);
    //}

    //public async Task<List<DailyDetailModel>> LoadAsync(
    //    BlobContainerClient blobClient, StormEventsDatabaseInventoryEntity eventsDatabase, CancellationToken ct)
    //{
    //    return await business.DatabaseLoadAsync(blobClient, eventsDatabase, ct);
    //}

    public async Task SourceDatabases(BlobContainerClient blobClient, CancellationToken ct)
    {
        var eventsList = await business.DatabaseList(ct);
        await business.DatabaseDownload(blobClient, eventsList, ct);
    }

    public Task<List<StormEventsDailySummaryEntity>> DeactivateOldSummaries(string id, int year, string sourceFk, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public List<DailySummaryModel> GetAggregate(List<DailyDetailModel> models)
    {
        throw new NotImplementedException();
    }

    public Task<List<DailyDetailModel>> Load(BlobContainerClient blobClient, StormEventsDatabaseEntity eventsDatabase, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}