using Azure.Storage.Blobs;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Satellite.Interfaces;
using olieblind.lib.Satellite.Models;
using olieblind.lib.Services;
using System.Diagnostics;

namespace olieblind.lib.Satellite;

public class SatelliteIemBusiness(ISatelliteSource source, ISatelliteIemSource iemSource, IOlieWebService ows, IMyRepository repo) : ISatelliteIemBusiness
{
    public async Task Download(SatelliteAwsProductEntity product, Func<int, Task> delayFunc, BlobContainerClient blobClient, CancellationToken ct)
    {
        if (product.PathSource is not null) return;

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var effectiveDate = source.GetEffectiveDate(product.EffectiveDate);
        var filename = Path.GetFileName(product.Id);
        var blobName = $"{source.GetPath(effectiveDate, "bronze")}/{filename}";
        var localFilename = OlieCommon.CreateLocalTmpPath(".tif");
        var key = $"{iemSource.GetPrefix(product.ScanTime)}{filename}";
        var attempt = 0;

        while (true)
        {
            try
            {
                var data = await ows.ApiGetBytes(key, ct);
                await ows.FileWriteAllBytes(localFilename, data, ct);
                break;
            }
            catch (Exception)
            {
                attempt++;
                if (attempt > 2) throw;
            }

            await delayFunc(attempt);
        }

        await ows.BlobUploadFile(blobClient, blobName, localFilename, ct);
        ows.FileDelete(localFilename);

        product.PathSource = blobName;
        product.TimeTakenDownload = (int)stopwatch.Elapsed.TotalSeconds;
        product.Timestamp = DateTime.UtcNow;
        await repo.SatelliteAwsProductUpdate(product, ct);
    }

    public async Task<AwsKeysModel?> ListKeys(string dayValue, int channel, DayPartsEnum dayPart,
        CancellationToken ct)
    {
        const string bucket = "IEM";

        var keys = new List<string>();

        var effectiveDate = source.GetEffectiveDate(dayValue);
        if (effectiveDate < new DateTime(2010, 1, 1)) return null;

        var start = source.GetEffectiveStart(effectiveDate, dayPart);
        var stop = source.GetEffectiveStop(effectiveDate, dayPart);
        start = start.AddMinutes(-start.Minute);
        stop = stop.AddMinutes(-stop.Minute).AddHours(1);

        var url = iemSource.GetPrefix(effectiveDate);
        var listFiles = await iemSource.IemList(url, ct);

        keys.AddRange(listFiles
            .Where(item =>
                channel == iemSource.GetChannelFromKey(item) &&
                iemSource.GetScanTimeFromKey(effectiveDate, item) >= start &&
                iemSource.GetScanTimeFromKey(effectiveDate, item) < stop));

        return new AwsKeysModel
        {
            Bucket = bucket,
            Keys = [.. keys.OrderBy(o => o)],
            GetScanTimeFunc = v => iemSource.GetScanTimeFromKey(effectiveDate, v)
        };
    }
}
