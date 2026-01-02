using Amazon.S3;
using Azure.Storage.Blobs;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.Radar.Interfaces;
using olieblind.lib.StormEvents;
using olieblind.lib.StormEvents.Interfaces;
using olieblind.lib.StormEvents.Models;

namespace olieblind.lib.Processes;

public class ImportStormEventsDatabaseProcess(
    IDatabaseProcess database,
    IDatabaseBusiness dbBusiness,
    IRadarBusiness radarBusiness,
    IMyRepository repo) : IImportStormEventsDatabaseProcess
{
    private List<RadarSiteEntity> _radarSites = [];
    private readonly List<RadarInventoryEntity> _radarInventory = [];

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

        await database.SourceDatabases(blobClient, ct);
        await ProcessEventsDatabases(year, update, blobClient, amazonClient, ct);
    }

    private async Task ProcessEventsDatabases(int year, string update, BlobContainerClient blobClient, AmazonS3Client amazonClient, CancellationToken ct)
    {
        var inventory = await repo.StormEventsDatabaseGet(year, update, ct)
                        ?? throw new InvalidOperationException($"No record for year {year}, update {update}");

        var events = await dbBusiness.DatabaseLoad(blobClient, inventory, ct);

        if (inventory.RowCount == 0)
        {
            await dbBusiness.DatabaseUpdateRowCount(inventory, events.Count, ct);
        }

        var start = new DateTime(year, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        var work = events
            .Where(w => w.Effective >= start)
            .Select(s => s.EffectiveDate)
            .Distinct()
            .OrderBy(o => o)
            .ToList();

        foreach (var effectiveDate in work)
        {
            var toProcess = events.Where(w => w.EffectiveDate == effectiveDate).ToList();
            await ProcessWorkItem(effectiveDate, year, update, toProcess, amazonClient, ct);
        }

        await dbBusiness.DatabaseUpdateActive(inventory, ct);
    }

    private async Task ProcessWorkItem(string effectiveDate, int year, string update, List<DailyDetailModel> models, AmazonS3Client amazonClient, CancellationToken ct)
    {
        var summaries = await database.DeactivateOldSummaries(effectiveDate, year, update, ct);
        var current = summaries.SingleOrDefault(s => s.SourceFk == update);

        if (current is not null && !current.IsCurrent)
        {
            await dbBusiness.CompareDetailCount(effectiveDate, update, current.RowCount, ct);
            await dbBusiness.ActivateSummary(current, ct);
        }
        else if (current is null)
        {
            await repo.StormEventsDailyDetailDelete(effectiveDate, update, ct);
            await AssignRadarAsync(models, amazonClient, ct);
            await dbBusiness.AddDailyDetailToCosmos(models, update, ct);
            var aggregate = DailySummaryBusiness.AggregateByDate(models);
            if (aggregate.Count != 1)
                throw new InvalidOperationException(
                    $"Got more than one aggregate for EffectiveDate: {effectiveDate}, Update: {update}");
            await dbBusiness.AddDailySummaryToCosmos(aggregate[0], update, ct);
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