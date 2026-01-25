using Azure.Messaging.ServiceBus.Administration;
using olieblind.lib.Satellite.Models;

namespace olieblind.lib.Satellite.Interfaces;

public interface ISatelliteRequestBusiness
{
    Task<SatelliteRequestStatisticsModel> GetStatistics(string userId, ServiceBusAdministrationClient client, CancellationToken ct);
}
