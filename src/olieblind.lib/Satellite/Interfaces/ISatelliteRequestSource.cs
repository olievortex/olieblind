using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using olieblind.data.Entities;
using olieblind.lib.Satellite.Models;

namespace olieblind.lib.Satellite.Interfaces;

public interface ISatelliteRequestSource
{
    Task CreateUserLog(string userId, string effectiveDate, bool isFree, CancellationToken ct);

    Task<List<SatelliteProductEntity>> GetHourlyProductList(string effectiveDate, CancellationToken ct);

    Task<SatelliteRequestStatisticsModel> GetRequestStatistics(string userId, ServiceBusAdministrationClient client, CancellationToken ct);

    Task<bool> IsFreeDay(string effectiveDate, string sourceFk, CancellationToken ct);

    Task<bool> IsQuotaAvailable(string userId, CancellationToken ct);

    Task SendMessage(List<SatelliteProductEntity> products, ServiceBusSender sender, CancellationToken ct);
}
