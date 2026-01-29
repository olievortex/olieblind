using olieblind.data.Enums;
using olieblind.lib.Satellite.Interfaces;
using olieblind.lib.Satellite.Sources;

namespace olieblind.lib.Satellite;

public class SatelliteImageProcess(ISatelliteImageBusiness business) : ISatelliteImageProcess
{
    public async Task DownloadInventory(string effectiveDate, int satellite, int channel, DayPartsEnum dayPart, ASatelliteSource source, CancellationToken ct)
    {
        var result = await source.ListKeys(effectiveDate, satellite, channel, dayPart, ct);
        if (result is null || result.Keys.Length == 0) return;

        await business.AddProductsToDatabase(result.Keys, effectiveDate, result.Bucket, channel, dayPart, result.GetScanTimeFunc, ct);
        await business.AddInventoryToDatabase(effectiveDate, result.Bucket, channel, dayPart, ct);
    }
}
