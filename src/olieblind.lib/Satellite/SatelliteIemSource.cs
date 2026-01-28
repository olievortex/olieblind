using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Satellite.Models;
using olieblind.lib.Services;
using System.Text.RegularExpressions;

namespace olieblind.lib.Satellite;

public partial class SatelliteIemSource : ASatelliteSource
{
    private const string UrlBase = "https://mesonet.agron.iastate.edu/archive/data/";

    public required IOlieWebService Ows { get; init; }

    public override async Task<(string, string)> Download(SatelliteProductEntity product, CancellationToken ct)
    {
        var effectiveDate = GetEffectiveDate(product.EffectiveDate);
        var filename = Path.GetFileName(product.Id);
        var blobName = $"{GetPath(effectiveDate, "bronze")}/{filename}";
        var localFilename = OlieCommon.CreateLocalTmpPath(".tif");
        var key = $"{GetPrefix(product.ScanTime)}{filename}";
        var attempt = 0;

        while (true)
        {
            try
            {
                var data = await Ows.ApiGetBytes(key, ct);
                await Ows.FileWriteAllBytes(localFilename, data, ct);
                break;
            }
            catch (Exception)
            {
                attempt++;
                if (attempt > 2) throw;
            }

            await DelayFunc(attempt);
        }

        return (blobName, localFilename);
    }

    public override async Task<SatelliteSourceKeysModel?> ListKeys(string dayValue, int satellite, int channel, DayPartsEnum dayPart, CancellationToken ct)
    {
        const string bucket = "IEM";

        var keys = new List<string>();

        var effectiveDate = GetEffectiveDate(dayValue);
        if (effectiveDate < new DateTime(2010, 1, 1)) return null;

        var start = GetEffectiveStart(effectiveDate, dayPart);
        var stop = GetEffectiveStop(effectiveDate, dayPart);
        start = start.AddMinutes(-start.Minute);
        stop = stop.AddMinutes(-stop.Minute).AddHours(1);

        var url = GetPrefix(effectiveDate);
        var listFiles = await IemList(url, ct);

        keys.AddRange(listFiles
            .Where(item =>
                channel == GetChannelFromKey(item) &&
                GetScanTimeFromKey(effectiveDate, item) >= start &&
                GetScanTimeFromKey(effectiveDate, item) < stop));

        return new SatelliteSourceKeysModel
        {
            Bucket = bucket,
            Keys = [.. keys.OrderBy(o => o)],
            GetScanTimeFunc = v => GetScanTimeFromKey(effectiveDate, v)
        };
    }

    public async Task<List<string>> IemList(string url, CancellationToken ct)
    {
        var html = await Ows.ApiGetString(url, ct);
        var items = ItemsRegex().Matches(html);

        var result = items
            .Select(s => s.Groups[1].Value)
            .ToList();

        return result;
    }

    public static int GetChannelFromKey(string value)
    {
        if (value.Contains("_ir4km_")) return 14;
        if (value.Contains("_vis4km_")) return 2;
        if (value.Contains("_wv4km_")) return 10;

        return -1;
    }

    public static string GetPrefix(DateTime effectiveDate)
    {
        var dateCode = $"{effectiveDate.Year}/{effectiveDate.Month:00}/{effectiveDate.Day:00}/";
        return $"{UrlBase}{dateCode}GIS/sat/";
    }

    public static DateTime GetScanTimeFromKey(DateTime effectiveDate, string value)
    {
        var filePart = Path.GetFileNameWithoutExtension(value);
        var timePart = filePart.Split('_')[3];

        var hours = int.Parse(timePart[..2]);
        var minutes = int.Parse(timePart[2..]);
        var offset = new TimeSpan(hours, minutes, 0);

        var result = effectiveDate.Add(offset);

        return result;
    }

    [GeneratedRegex("<a href=\"(\\w\\S+)\"")]
    private static partial Regex ItemsRegex();
}
