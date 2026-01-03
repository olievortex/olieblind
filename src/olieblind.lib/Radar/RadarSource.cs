using Amazon.S3;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.Mapping;
using olieblind.lib.Radar.Interfaces;
using olieblind.lib.Services;

namespace olieblind.lib.Radar;

public class RadarSource(IMyRepository repo, IOlieWebService ows) : IRadarSource
{
    public const string LevelIiBucket = "unidata-nexrad-level2";

    public async Task<RadarInventoryEntity> AddRadarInventory(List<RadarInventoryEntity> cache, RadarSiteEntity radar,
        DateTime effectiveTime, AmazonS3Client client, CancellationToken ct)
    {
        var prefix = $"{effectiveTime:yyyy}/{effectiveTime:MM}/{effectiveTime:dd}/{radar.Id}/";
        var products = await ows.AwsList(LevelIiBucket, prefix, client, ct);

        var entity = new RadarInventoryEntity
        {
            BucketName = LevelIiBucket,
            EffectiveDate = $"{effectiveTime:yyyy-MM-dd}",
            FileList = products,
            Id = radar.Id,
            Timestamp = DateTime.UtcNow
        };

        EntityCompression.Compress(entity);

        await repo.RadarInventoryAdd(entity, ct);
        cache.Add(entity);

        return entity;
    }

    public List<RadarSiteEntity> FindClosestRadars(List<RadarSiteEntity> radarSites, double lat, double lon)
    {
        var closest = radarSites
            .Select(s => new
            {
                s,
                Distance = Math.Sqrt(Math.Pow(s.Latitude - lat, 2) + Math.Pow(s.Longitude - lon, 2))
            })
            .OrderBy(o => o.Distance)
            .Select(s => s.s)
            .Take(4)
            .ToList();

        return closest;
    }

    public async Task<RadarInventoryEntity?> GetRadarInventory(List<RadarInventoryEntity> cache,
        RadarSiteEntity radar, DateTime effectiveTime, CancellationToken ct)
    {
        var effectiveDate = $"{effectiveTime:yyyy-MM-dd}";
        var result = cache.SingleOrDefault(a => a.BucketName == LevelIiBucket &&
                                                a.EffectiveDate == effectiveDate &&
                                                a.Id == radar.Id);

        if (result is not null) return result;

        result = await repo.RadarInventoryGet(radar.Id, effectiveDate, LevelIiBucket, ct);

        if (result is null) return null;

        cache.Add(result);

        return result;
    }
}