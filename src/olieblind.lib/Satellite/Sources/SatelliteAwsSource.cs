using Amazon.S3;
using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Satellite.Models;

namespace olieblind.lib.Satellite.Sources;

public class SatelliteAwsSource : ASatelliteSource
{
    public required IAmazonS3 AmazonS3Client { get; init; }

    public override async Task<(string, string)> Download(SatelliteProductEntity product, Func<int, Task> delayFunc, CancellationToken ct)
    {
        var effectiveDate = GetEffectiveDate(product.EffectiveDate);
        var filename = Path.GetFileName(product.Id);
        var blobName = $"{GetPath(effectiveDate, "bronze")}/{filename}";
        var localFilename = $"{Path.GetTempPath()}{filename}";
        var key = $"{GetPrefix(product.ScanTime)}{filename}";
        var attempt = 0;

        while (true)
        {
            try
            {
                await Ows.AwsDownload(localFilename, product.BucketName, key, AmazonS3Client, ct);
                break;
            }
            catch (AmazonS3Exception)
            {
                attempt++;
                if (attempt > 2) throw;
            }

            await delayFunc(attempt);
        }

        return (blobName, localFilename);
    }

    public override async Task<SatelliteSourceKeysModel?> ListKeys(string dayValue, int satellite, int channel, DayPartsEnum dayPart, CancellationToken ct)
    {
        var effectiveDate = GetEffectiveDate(dayValue);
        if (effectiveDate < new DateTime(2018, 1, 1)) return null;

        var start = GetEffectiveStart(effectiveDate, dayPart);
        var stop = GetEffectiveStop(effectiveDate, dayPart);
        var keys = new List<string>();
        var bucketName = GetBucketName(satellite);
        var startLoop = start.AddMinutes(-start.Minute);
        var finishLoop = stop.AddMinutes(-stop.Minute).AddHours(1);

        while (startLoop < finishLoop)
        {
            var prefix = GetPrefix(startLoop);
            var listFiles = await Ows.AwsList(bucketName, prefix, AmazonS3Client, ct);

            keys.AddRange(listFiles
                .Where(item => channel == GetChannelFromAwsKey(item)));

            startLoop = startLoop.AddHours(1);
        }

        return new SatelliteSourceKeysModel
        {
            Bucket = GetBucketName(satellite),
            Keys = [.. keys.OrderBy(o => o)],
            GetScanTimeFunc = GetScanTime
        };
    }

    public static string GetBucketName(int satellite)
    {
        return $"noaa-goes{satellite}";
    }

    public static int GetChannelFromAwsKey(string key)
    {
        var fileName = key.Split('/')[^1];
        var channel = int.Parse(fileName.Split('_')[1][^2..]);

        return channel;
    }

    public static string GetPrefix(DateTime effectiveHour)
    {
        var dateTimeFolder = $"{effectiveHour:yyyy}/{effectiveHour.DayOfYear:000}/{effectiveHour:HH}/";
        return $"ABI-L1b-RadC/{dateTimeFolder}";
    }

    public static DateTime GetScanTime(string filename)
    {
        // OR_ABI-L1b-RadF-M3C02_G16_s20171671145342_e20171671156109_c20171671156144.nc
        var parts = filename.Split('_');
        var created = parts[3][1..];

        var year = int.Parse(created[..4]);
        var dayNumber = int.Parse(created[4..7]);
        var hour = int.Parse(created[7..9]);
        var minute = int.Parse(created[9..11]);
        var second = int.Parse(created[11..13]);
        var millisecond = (created[13] - '0') * 100;

        var result = new DateTime(year, 1, 1, hour, minute, second, millisecond, DateTimeKind.Utc)
            .AddDays(dayNumber - 1);

        return result;
    }
}
