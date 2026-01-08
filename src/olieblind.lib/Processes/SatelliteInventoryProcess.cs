using Amazon.S3;
using olieblind.data.Enums;
using olieblind.lib.Processes.Interfaces;
using olieblind.lib.Satellite.Interfaces;
using olieblind.lib.StormEvents.Interfaces;

namespace olieblind.lib.Processes;

public class SatelliteInventoryProcess(
    IDailySummaryBusiness stormy,
    ISatelliteProcess process,
    ISatelliteSource source) : ISatelliteInventoryProcess
{
    private const int Channel = 2;
    private const DayPartsEnum DayPart = DayPartsEnum.Afternoon;
    private const string Goes19 = "2025-04-07";

    public async Task Run(int year, IAmazonS3 client, CancellationToken ct)
    {
        var missingDays = await GetMissingDaysAsync(year, ct);

        foreach (var missingDay in missingDays)
        {
            var satellite = string.Compare(missingDay, Goes19, StringComparison.Ordinal) < 1 ? 16 : 19;
            await process.ProcessMissingDayAsync(year, missingDay, satellite, Channel, DayPart, client, ct);
        }
    }

    public async Task<List<string>> GetMissingDaysAsync(int year, CancellationToken ct)
    {
        var stormDays = (await stormy.GetSevereByYear(year, ct))
            .Select(s => s.Id)
            .ToList();
        var inventoryDays = (await source.GetInventoryByYearAsync(year, Channel, DayPart, ct))
            .Select(s => s.EffectiveDate)
            .ToList();

        var missingDays = stormDays.Except(inventoryDays).ToList();

        return missingDays;
    }
}