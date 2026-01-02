using Amazon.S3;
using olieblind.data.Entities;

namespace olieblind.lib.Radar.Interfaces;

public interface IRadarSource
{
    Task<RadarInventoryEntity> AddRadarInventory(List<RadarInventoryEntity> cache, RadarSiteEntity radar, DateTime effectiveTime,
        AmazonS3Client client, CancellationToken ct);

    List<RadarSiteEntity> FindClosestRadars(List<RadarSiteEntity> radarSites, double lat, double lon);

    Task<RadarInventoryEntity?> GetRadarInventory(List<RadarInventoryEntity> cache, RadarSiteEntity radar,
        DateTime effectiveTime, CancellationToken ct);
}