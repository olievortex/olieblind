using Azure.Messaging.ServiceBus;
using olieblind.data.Entities;

namespace olieblind.lib.Satellite.Interfaces;

public interface ISatelliteRequestBusiness
{
    Task CreateLog(string userId, string effectiveDate, bool isFree, CancellationToken ct);

    Task Enqueue(List<SatelliteAwsProductEntity> products, ServiceBusSender sender, CancellationToken ct);

    Task<List<SatelliteAwsProductEntity>> GetHourlyProductList(string effectiveDate, CancellationToken ct);

    Task<bool> IsFreeRequest(string effectiveDate, string sourceFk, CancellationToken ct);

    Task<bool> IsQuotaAvailable(string userId, CancellationToken ct);
}
