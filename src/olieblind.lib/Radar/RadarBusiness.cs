using Amazon.S3;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.Radar.Interfaces;

namespace olieblind.lib.Radar;

public class RadarBusiness(IRadarSource source, IMyRepository repo) : IRadarBusiness
{
    public async Task<RadarSiteEntity> DownloadInventoryForClosestRadar(List<RadarSiteEntity> radarSites,
        List<RadarInventoryEntity> cache, DateTime effectiveTime, double latitude, double longitude,
        AmazonS3Client client, CancellationToken ct)
    {
        foreach (var radarSite in source.FindClosestRadars(radarSites, latitude, longitude))
        {
            var inventory = await DownloadInventory(radarSite, effectiveTime);
            if (inventory.FileList.Count == 0) continue;

            await DownloadInventory(radarSite, effectiveTime.AddHours(-3));
            await DownloadInventory(radarSite, effectiveTime.AddHours(1));

            return radarSite;
        }

        throw new InvalidOperationException($"Unable to find a radar for {latitude}, {longitude}");

        async Task<RadarInventoryEntity> DownloadInventory(RadarSiteEntity radarSite, DateTime timeValue)
        {
            var inventory = await source.GetRadarInventory(cache, radarSite, timeValue, ct);
            inventory ??= await source.AddRadarInventory(cache, radarSite, timeValue, client, ct);

            return inventory;
        }
    }

    public async Task<List<RadarSiteEntity>> GetPrimaryRadarSites(CancellationToken ct)
    {
        return [.. (await repo.RadarSiteGetAll(ct)).Where(w => w.Id.StartsWith('K'))];
    }

    public async Task PopulateRadarSitesFromCsv(string csv, CancellationToken ct)
    {
        var lines = csv.ReplaceLineEndings("\n").Split('\n');
        var lineNumber = 0;
        var radars = new List<RadarSiteEntity>();

        foreach (var line in lines)
        {
            // Skip the header
            lineNumber++;
            if (lineNumber < 3) continue;
            if (string.IsNullOrEmpty(line)) continue;

            // Parse the parts
            var icao = line[9..13];
            var name = line[20..50].Trim();
            var state = line[72..74].Trim();
            var lat = double.Parse(line[106..115]);
            var lon = double.Parse(line[116..126]);
            var type = line[140..146];
            if (string.IsNullOrWhiteSpace(state)) continue;
            if (type != "NEXRAD") continue;

            // Create the record
            var entity = new RadarSiteEntity
            {
                Id = icao,

                Name = name,
                State = state,
                Latitude = lat,
                Longitude = lon,
                Timestamp = DateTime.UtcNow
            };

            radars.Add(entity);
        }

        await repo.RadarSiteCreate(radars, ct);
    }
}