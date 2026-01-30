using Amazon.S3;
using olieblind.data;
using olieblind.data.Enums;
using olieblind.lib.Processes.Interfaces;
using olieblind.lib.Satellite.Interfaces;
using olieblind.lib.Satellite.Sources;
using olieblind.lib.StormEvents.Interfaces;

namespace olieblind.lib.Processes;

public class SatelliteInventoryProcess(
    IDailySummaryBusiness stormy,
    ISatelliteImageBusiness business,
    IMyRepository repo) : ISatelliteInventoryProcess
{
    private const int Channel = 2;
    private const DayPartsEnum DayPart = DayPartsEnum.Afternoon;
    private const string Goes19 = "2025-04-07";

    public async Task Run(int year, IAmazonS3 client, CancellationToken ct)
    {
        var missingDays = await GetMissingDays(year, ct);
        var source = business.CreateSatelliteSource(year, client);

        foreach (var missingDay in missingDays)
        {
            var satellite = SelectSatellite(missingDay);
            await DownloadInventory(missingDay, satellite, Channel, DayPart, source, ct);
        }
    }

    public static int SelectSatellite(string effectiveDate) => string.Compare(effectiveDate, Goes19, StringComparison.Ordinal) < 1 ? 16 : 19;

    public async Task DownloadInventory(string effectiveDate, int satellite, int channel, DayPartsEnum dayPart, ASatelliteSource source, CancellationToken ct)
    {
        var result = await source.ListKeys(effectiveDate, satellite, channel, dayPart, ct);
        if (result is null || result.Keys.Length == 0) return;

        await business.AddProductsToDatabase(result.Keys, effectiveDate, result.Bucket, channel, dayPart, result.GetScanTimeFunc, ct);
        await business.AddInventoryToDatabase(effectiveDate, result.Bucket, channel, dayPart, ct);
    }

    public async Task<List<string>> GetMissingDays(int year, CancellationToken ct)
    {
        var stormDays = (await stormy.GetSevereByYear(year, ct))
            .Select(s => s.Id)
            .ToList();
        var inventoryDays = (await repo.SatelliteInventoryListByYear(year, Channel, DayPart, ct))
            .Select(s => s.EffectiveDate)
            .ToList();

        var missingDays = stormDays.Except(inventoryDays).ToList();

        return missingDays;
    }
}