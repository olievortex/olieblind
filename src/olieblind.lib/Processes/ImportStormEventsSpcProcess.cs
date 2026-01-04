using Amazon.S3;
using olieblind.data.Entities;
using olieblind.lib.Processes.Interfaces;
using olieblind.lib.Radar.Interfaces;
using olieblind.lib.StormEvents.Interfaces;
using olieblind.lib.StormEvents.Models;

namespace olieblind.lib.Processes;

public class ImportStormEventsSpcProcess(
    ISpcProcess spc,
    ISpcSource spcSource,
    IRadarBusiness radarBusiness) : IImportStormEventsSpcProcess
{
    private List<RadarSiteEntity> _radarSites = [];
    private readonly List<RadarInventoryEntity> _radarInventory = [];

    public async Task Run(int year, AmazonS3Client client, CancellationToken ct)
    {
        _radarSites = await radarBusiness.GetPrimaryRadarSites(ct);

        var (start, stop, inventoryYear) = await spc.GetInventoryByYear(year, ct);
        var cutoff = DateTime.UtcNow.AddDays(-2).Date;

        for (var day = start; day <= stop; day++)
        {
            var effectiveDate = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays(day);
            if (effectiveDate > cutoff) break;

            var inventory = await spc.SourceInventory(effectiveDate, inventoryYear, ct);
            if (spc.ShouldSkip(inventory)) continue;

            var events = spcSource.Parse(effectiveDate, inventory.Rows);
            await AssignRadarAsync(events, client, ct);

            await spc.ProcessEvents(events, inventory, ct);
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