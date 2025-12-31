using Amazon.S3;
using olieblind.data.Entities;

namespace olieblind.lib.Radar.Interfaces;

public interface IRadarBusiness
{
    Task<RadarSiteEntity> DownloadInventoryForClosestRadarAsync(List<RadarSiteEntity> radarSites,
        List<RadarInventoryEntity> cache, DateTime effectiveTime, double latitude, double longitude,
        AmazonS3Client client, CancellationToken ct);

    Task PopulateRadarSitesFromCsvAsync(string csv, CancellationToken ct);
}