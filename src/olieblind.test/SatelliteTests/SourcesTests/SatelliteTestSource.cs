using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Satellite.Models;
using olieblind.lib.Satellite.Sources;

namespace olieblind.test.SatelliteTests.SourcesTests;

internal class SatelliteTestSource : ASatelliteSource
{
    public override Task<(string, string)> Download(SatelliteProductEntity product, Func<int, Task> funcDelay, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async override Task<SatelliteSourceKeysModel?> ListKeys(string dayValue, int satellite, int channel, DayPartsEnum dayPart, CancellationToken ct)
    {
        return new SatelliteSourceKeysModel
        {
            Keys = channel == 99 ? [] : ["a, b"],
            GetScanTimeFunc = (s) => DateTime.UtcNow
        };
    }
}
