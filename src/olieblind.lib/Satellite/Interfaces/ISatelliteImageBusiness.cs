using olieblind.data.Entities;
using olieblind.data.Enums;
using SixLabors.ImageSharp;

namespace olieblind.lib.Satellite.Interfaces;

public interface ISatelliteImageBusiness
{
    Task AddInventoryToDatabase(string effectiveDate, string bucket, int channel, DayPartsEnum dayPart, CancellationToken ct);

    Task AddProductsToDatabase(string[] keys, string effectiveDate, string bucket, int channel, DayPartsEnum dayPart, Func<string, DateTime> scanTimeFunc, CancellationToken ct);

    Task<SatelliteProductEntity?> GetMarqueeSatelliteProduct(string effectiveDate, DateTime eventTime, CancellationToken ct);

    Task MakeThumbnail(SatelliteProductEntity satellite, Point finalSize, string goldPath, CancellationToken ct);
}
