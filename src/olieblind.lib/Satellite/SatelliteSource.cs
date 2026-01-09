using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Satellite.Interfaces;
using SixLabors.ImageSharp;

namespace olieblind.lib.Satellite;

public class SatelliteSource(IMyRepository repo) : ISatelliteSource
{
    public string GetPath(DateTime effectiveDate, string metal)
    {
        throw new NotImplementedException();
    }

    public Task<List<SatelliteAwsProductEntity>> GetProductListAsync(string effectiveDate, string bucketName, int channel, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<List<SatelliteAwsProductEntity>> GetProductListNoPosterAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<SatelliteAwsProductEntity?> GetProductPosterAsync(string effectiveDate, DateTime eventTime, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task MakePosterAsync(SatelliteAwsProductEntity satellite, Point finalSize, BlobContainerClient goldClient, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<bool> MessagePurpleAsync(SatelliteAwsProductEntity satellite, ServiceBusSender sender, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task Start1080ContainersAsync(DefaultAzureCredential credential, int instanceLimit, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task AddInventoryToDatabase(string effectiveDate, string bucket, int channel, DayPartsEnum dayPart,
       CancellationToken ct)
    {
        var entity = new SatelliteAwsInventoryEntity
        {
            Id = bucket,
            EffectiveDate = effectiveDate,

            Channel = channel,
            DayPart = dayPart,
            Timestamp = DateTime.UtcNow
        };

        await repo.SatelliteAwsInventoryCreate(entity, ct);
    }

    public async Task AddProductsToDatabase(string[] keys, string effectiveDate, string bucket, int channel,
        DayPartsEnum dayPart, Func<string, DateTime> getScanTimeFunc, CancellationToken ct)
    {
        var items = new List<SatelliteAwsProductEntity>();

        foreach (var key in keys)
        {
            var entity = new SatelliteAwsProductEntity
            {
                Id = Path.GetFileName(key),
                EffectiveDate = effectiveDate,

                BucketName = bucket,
                Channel = channel,
                DayPart = dayPart,
                Path1080 = null,
                PathPoster = null,
                PathSource = null,
                ScanTime = getScanTimeFunc(key),
                Timestamp = DateTime.UtcNow,
                TimeTaken1080 = 0,
                TimeTakenDownload = 0,
                TimeTakenPoster = 0
            };

            items.Add(entity);
        }

        await repo.SatelliteAwsProductCreate(items, ct);
    }

    public DateTime GetEffectiveDate(string value)
    {
        var parts = value.Split('-').Select(int.Parse).ToArray();
        var effectiveDate = new DateTime(parts[0], parts[1], parts[2], 0, 0, 0,
            DateTimeKind.Utc);

        return effectiveDate;
    }

    public DateTime GetEffectiveStart(DateTime effectiveDate, DayPartsEnum dayPart)
    {
        return dayPart switch
        {
            DayPartsEnum.Owl => effectiveDate.Date.AddHours(6),
            DayPartsEnum.Morning => effectiveDate.Date.AddHours(12),
            DayPartsEnum.Afternoon => effectiveDate.Date.AddHours(18),
            _ => effectiveDate.Date.AddHours(24)
        };
    }

    public DateTime GetEffectiveStop(DateTime effectiveDate, DayPartsEnum dayPart)
    {
        return dayPart switch
        {
            DayPartsEnum.Owl => effectiveDate.Date.AddHours(11),
            DayPartsEnum.Morning => effectiveDate.Date.AddHours(17),
            DayPartsEnum.Afternoon => effectiveDate.Date.AddHours(23),
            _ => effectiveDate.Date.AddHours(29)
        };
    }

    //public string GetPath(DateTime effectiveDate, string metal)
    //{
    //    var pathDate = effectiveDate.ToString("yyyy/MM/dd");
    //    var path = $"{metal}/aws/satellite/{pathDate}";

    //    return path;
    //}

    //public async Task<SatelliteAwsProductEntity?> GetProductPosterAsync(string effectiveDate, DateTime eventTime,
    //    CancellationToken ct)
    //{
    //    var result =
    //        await cosmos.SatelliteAwsProductGetPosterAsync(effectiveDate, eventTime, ct) ??
    //        await cosmos.SatelliteAwsProductGetLastPosterAsync(effectiveDate, ct);

    //    return result;
    //}

    //public async Task<List<SatelliteAwsProductEntity>> GetProductListAsync(string effectiveDate, string bucketName,
    //    int channel, CancellationToken ct)
    //{
    //    return await cosmos.SatelliteAwsProductListAsync(effectiveDate, bucketName, channel, ct);
    //}

    //public async Task<List<SatelliteAwsProductEntity>> GetProductListNoPosterAsync(CancellationToken ct)
    //{
    //    return await cosmos.SatelliteAwsProductListNoPosterAsync(ct);
    //}

    //public async Task MakePosterAsync(SatelliteAwsProductEntity satellite, Point finalSize,
    //    BlobContainerClient goldClient, CancellationToken ct)
    //{
    //    var stopwatch = new Stopwatch();
    //    stopwatch.Start();

    //    // Sanity check
    //    if (satellite.PathPoster is not null) return;
    //    if (satellite.Path1080 is null) throw new NullReferenceException($"Missing Path1080 for {satellite.Id}");

    //    // Download full sized image
    //    var filename1080 = CommonProcess.CreateLocalTmpPath(".png");
    //    await ows.BlobDownloadFileAsync(goldClient, satellite.Path1080, filename1080, ct);
    //    var bytes = await ows.FileReadAllBytes(filename1080, ct);

    //    // Convert to poster image
    //    var filenamePoster = CommonProcess.CreateLocalTmpPath(".png");
    //    var resizedBytes = await ois.ResizeAsync(bytes, finalSize, ct);
    //    await ows.FileWriteAllBytesAsync(filenamePoster, resizedBytes, ct);

    //    // Save to blob storage
    //    var blobPoster = satellite.Path1080.Replace(".png", "_poster.png");
    //    await ows.BlobUploadFileAsync(goldClient, blobPoster, filenamePoster, ct);

    //    // Update CosmosDb
    //    satellite.PathPoster = blobPoster;
    //    satellite.Timestamp = DateTime.UtcNow;
    //    satellite.TimeTakenPoster = (int)stopwatch.Elapsed.TotalSeconds;
    //    await cosmos.SatelliteAwsProductUpdateAsync(satellite, ct);

    //    // Cleanup
    //    ows.FileDelete(filename1080);
    //    ows.FileDelete(filenamePoster);
    //}

    //public async Task<bool> MessagePurpleAsync(SatelliteAwsProductEntity satellite, ServiceBusSender sender,
    //    CancellationToken ct)
    //{
    //    if (satellite.Path1080 is not null || satellite.PathSource is null) return false;

    //    await ows.ServiceBusSendJsonAsync(sender, satellite, ct);
    //    return true;
    //}

    //public async Task Start1080ContainersAsync(DefaultAzureCredential credential, int instanceLimit,
    //    CancellationToken ct)
    //{
    //    var startInfo = new ContainerStartInfo
    //    {
    //        Credential = credential,
    //        ContainerGroupNameStart = "olievortex-purple-",
    //        ResourceGroupName = "rg-container",
    //        SubscriptionId = "/subscriptions/bfc1fbca-c3f0-48a2-9263-9ebb68c8ed96"
    //    };

    //    await ows.StartContainerGroupsAsync(startInfo, instanceLimit, ct);
    //}
}
