using Amazon.S3;
using Azure.Storage.Blobs;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Satellite.Interfaces;
using olieblind.lib.Satellite.Models;
using olieblind.lib.Services;
using System.Diagnostics;

namespace olieblind.lib.Satellite;

public class SatelliteAwsBusiness(ISatelliteSource source, ISatelliteAwsSource awsSource, IOlieWebService ows, IMyRepository repo) : ISatelliteAwsBusiness
{
    public async Task Download(SatelliteAwsProductEntity product, Func<int, Task> delayFunc,
        BlobContainerClient blobClient, IAmazonS3 awsClient, CancellationToken ct)
    {
        if (product.PathSource is not null) return;

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var effectiveDate = source.GetEffectiveDate(product.EffectiveDate);
        var filename = Path.GetFileName(product.Id);
        var blobName = $"{source.GetPath(effectiveDate, "bronze")}/{filename}";
        var localFilename = $"{Path.GetTempPath()}{filename}";
        var key = $"{awsSource.GetPrefix(product.ScanTime)}{filename}";
        var attempt = 0;

        while (true)
        {
            try
            {
                await ows.AwsDownload(localFilename, product.BucketName, key, awsClient, ct);
                break;
            }
            catch (AmazonS3Exception)
            {
                attempt++;
                if (attempt > 2) throw;
            }

            await delayFunc(attempt);
        }

        await ows.BlobUploadFile(blobClient, blobName, localFilename, ct);

        product.PathLocal = localFilename;
        product.PathSource = blobName;
        product.TimeTakenDownload = (int)stopwatch.Elapsed.TotalSeconds;
        product.Timestamp = DateTime.UtcNow;
        await repo.SatelliteAwsProductUpdate(product, ct);
    }

    public async Task<AwsKeysModel?> ListKeys(string dayValue, int satellite, int channel,
        DayPartsEnum dayPart, IAmazonS3 client, CancellationToken ct)
    {
        var effectiveDate = source.GetEffectiveDate(dayValue);
        if (effectiveDate < new DateTime(2018, 1, 1)) return null;

        var start = source.GetEffectiveStart(effectiveDate, dayPart);
        var stop = source.GetEffectiveStop(effectiveDate, dayPart);
        var keys = new List<string>();
        var bucketName = awsSource.GetBucketName(satellite);
        var startLoop = start.AddMinutes(-start.Minute);
        var finishLoop = stop.AddMinutes(-stop.Minute).AddHours(1);

        while (startLoop < finishLoop)
        {
            var prefix = awsSource.GetPrefix(startLoop);
            var listFiles = await ows.AwsList(bucketName, prefix, client, ct);

            keys.AddRange(listFiles
                .Where(item => channel == awsSource.GetChannelFromAwsKey(item)));

            startLoop = startLoop.AddHours(1);
        }

        return new AwsKeysModel
        {
            Bucket = awsSource.GetBucketName(satellite),
            Keys = [.. keys.OrderBy(o => o)],
            GetScanTimeFunc = awsSource.GetScanTime
        };
    }
}
