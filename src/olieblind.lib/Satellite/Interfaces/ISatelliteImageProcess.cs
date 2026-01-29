using olieblind.data.Enums;
using olieblind.lib.Satellite.Sources;

namespace olieblind.lib.Satellite.Interfaces;

public interface ISatelliteImageProcess
{
    Task DownloadInventory(string effectiveDate, int satellite, int channel, DayPartsEnum dayPart, ASatelliteSource source, CancellationToken ct);
}