using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using olieblind.data.Entities;
using olieblind.data.Enums;
using SixLabors.ImageSharp;

namespace olieblind.lib.Satellite.Interfaces;

public interface ISatelliteSource
{
    Task AddInventoryToDatabase(string effectiveDate, string bucket, int channel, DayPartsEnum dayPart, CancellationToken ct);

    Task AddProductsToDatabase(string[] keys, string effectiveDate, string bucket, int channel, DayPartsEnum dayPart,
        Func<string, DateTime> getScanTimeFunc, CancellationToken ct);

    DateTime GetEffectiveDate(string value);

    DateTime GetEffectiveStart(DateTime effectiveDate, DayPartsEnum dayPart);

    DateTime GetEffectiveStop(DateTime effectiveDate, DayPartsEnum dayPart);

    string GetPath(DateTime effectiveDate, string metal);

    Task<SatelliteAwsProductEntity?> GetMarqueeSatelliteProduct(string effectiveDate, DateTime eventTime, CancellationToken ct);

    Task MakeThumbnail(SatelliteAwsProductEntity satellite, Point finalSize, string goldPath, CancellationToken ct);

    Task MessagePurple(SatelliteAwsProductEntity satellite, ServiceBusSender sender, CancellationToken ct);
}