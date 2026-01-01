using Amazon.S3;
using olieblind.data.Entities;

namespace olieblind.lib.Radar.Interfaces;

public interface IRadarBusiness
{
    Task<RadarSiteEntity> DownloadInventoryForClosestRadar(List<RadarSiteEntity> radarSites,
        List<RadarInventoryEntity> cache, DateTime effectiveTime, double latitude, double longitude,
        AmazonS3Client client, CancellationToken ct);

    Task<List<RadarSiteEntity>> GetPrimaryRadarSites(CancellationToken ct);

    Task PopulateRadarSitesFromCsv(string csv, CancellationToken ct);
}