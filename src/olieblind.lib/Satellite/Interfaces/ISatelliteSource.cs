using Azure.Messaging.ServiceBus;
using olieblind.data.Entities;
using SixLabors.ImageSharp;

namespace olieblind.lib.Satellite.Interfaces;

public interface ISatelliteSource
{
    Task<SatelliteProductEntity?> GetMarqueeSatelliteProduct(string effectiveDate, DateTime eventTime, CancellationToken ct);

    Task MakeThumbnail(SatelliteProductEntity satellite, Point finalSize, string goldPath, CancellationToken ct);

    Task MessagePurple(SatelliteProductEntity satellite, ServiceBusSender sender, CancellationToken ct);
}