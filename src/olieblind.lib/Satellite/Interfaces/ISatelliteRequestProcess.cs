using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using olieblind.lib.Models;
using olieblind.lib.Satellite.Models;

namespace olieblind.lib.Satellite.Interfaces;

public interface ISatelliteRequestProcess
{
    Task<SatelliteRequestStatisticsModel> GetStatistics(string userId, ServiceBusAdministrationClient client, CancellationToken ct);

    Task<SatelliteRequestResultModel> RequestHourlySatellite(SatelliteRequestModel model, ServiceBusSender sender, CancellationToken ct);
}
