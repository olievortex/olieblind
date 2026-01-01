using Amazon.S3;
using Azure.Storage.Blobs;
using olieblind.data.Entities;
using olieblind.lib.Radar.Interfaces;
using olieblind.lib.StormEvents.Interfaces;
using olieblind.lib.StormEvents.Models;

namespace olieblind.lib.Processes;

public class ImportStormEventsDatabaseProcess(
    IDatabaseProcess database,
    IDatabaseBusiness dbBusiness,
    IRadarBusiness radarBusiness) : IImportStormEventsDatabaseProcess
{
    private List<RadarSiteEntity> _radarSites = [];
    private readonly List<RadarInventoryEntity> _radarInventory = [];
    private int _dayCount;

    /// <summary>
    /// 1. Download the Storm Events inventory list
    /// 2. Download any updates and upload them to blob storage.
    /// 3. Parse the storm events database for the given year.
    /// 4. vents, aggregate, and determine the closest radar for each event.
    /// </summary>
    /// <param name="year">The year for which to initialize and process radar site data.</param>
    /// <param name="blobClient">A BlobContainerClient instance used to access blob storage for sourcing databases.</param>
    /// <param name="amazonClient">An AmazonS3Client instance used to access Amazon S3 storage. This parameter is reserved for future use.</param>
    /// <param name="ct">A CancellationToken that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Run(int year, string update, BlobContainerClient blobClient, AmazonS3Client amazonClient, CancellationToken ct)
    {
        _radarSites = await radarBusiness.GetPrimaryRadarSites(ct);
        _radarInventory.Clear();
        _dayCount = 0;

        await database.SourceDatabases(blobClient, ct);
        await ProcessEventsDatabases(year, update, blobClient, amazonClient, ct);
    }

    private async Task ProcessEventsDatabases(int year, string update, BlobContainerClient blobClient, AmazonS3Client amazonClient, CancellationToken ct)
    {
        var inventory = await dbBusiness.DatabaseGetInventory(year, update, ct)
                        ?? throw new InvalidOperationException($"No record for year {year}, update {update}");

        //var events = await database.LoadAsync(blobClient, inventory, ct);

        //if (inventory.RowCount == 0)
        //{
        //    await dbBusiness.DatabaseUpdateRowCountAsync(inventory, events.Count, ct);
        //    inventory.RowCount = events.Count;
        //}

        //var start = new DateTime(year, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        //var work = events
        //    .Where(w => w.Effective >= start)
        //    .Select(s => s.EffectiveDate)
        //    .Distinct()
        //    .OrderBy(o => o)
        //    .ToList();

        //foreach (var workItem in work)
        //{
        //    var toProcess = events.Where(w => w.EffectiveDate == workItem).ToList();
        //    await ProcessWorkItemAsync(workItem, year, update, toProcess, amazonClient, ct);

        //    if (_dayCount > 31) return;
        //}

        //var result = _dayCount > 0;

        //if (!result) await dbBusiness.DatabaseUpdateActiveAsync(inventory, ct);
    }

    private async Task ProcessWorkItem(string id, int year, string sourceFk, List<DailyDetailModel> models,
        AmazonS3Client amazonClient, CancellationToken ct)
    {
        Console.WriteLine($"Id: {id}, SourceFk: {sourceFk}");

        var summaries = await database.DeactivateOldSummaries(id, year, sourceFk, ct);
        var current = summaries.SingleOrDefault(s => s.SourceFk == sourceFk);

        if (current is not null && !current.IsCurrent)
        {
            await dbBusiness.CompareDetailCount(id, sourceFk, current.RowCount, ct);
            await dbBusiness.ActivateSummary(current, ct);
            if (_dayCount == 0) _dayCount++;
        }
        else if (current is null)
        {
            await dbBusiness.DeleteDetail(id, sourceFk, ct);
            await AssignRadarAsync(models, amazonClient, ct);
            await dbBusiness.AddDailyDetailToCosmos(models, sourceFk, ct);
            var aggregate = database.GetAggregate(models);
            if (aggregate.Count != 1)
                throw new InvalidOperationException(
                    $"Got more than one aggregate for dateFk: {id}, sourceFk: {sourceFk}");
            await dbBusiness.AddDailySummaryToCosmos(aggregate[0], sourceFk, ct);
            _dayCount++;
        }
    }

    private async Task AssignRadarAsync(List<DailyDetailModel> stormEvents, AmazonS3Client client, CancellationToken ct)
    {
        foreach (var stormEvent in stormEvents)
        {
            var radarSite = await radarBusiness.DownloadInventoryForClosestRadar(_radarSites, _radarInventory,
                stormEvent.Effective, stormEvent.Latitude, stormEvent.Longitude, client, ct);
            stormEvent.ClosestRadar = radarSite.Id;
        }
    }
}