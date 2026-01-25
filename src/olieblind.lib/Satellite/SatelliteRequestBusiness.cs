using Azure.Messaging.ServiceBus.Administration;
using olieblind.data;
using olieblind.lib.Satellite.Interfaces;
using olieblind.lib.Satellite.Models;
using olieblind.lib.Services;

namespace olieblind.lib.Satellite;

public class SatelliteRequestBusiness(IOlieWebService ows, IOlieConfig config, IMyRepository repo) : ISatelliteRequestBusiness
{
    public async Task<SatelliteRequestStatisticsModel> GetStatistics(string userId, ServiceBusAdministrationClient client, CancellationToken ct)
    {
        var stats = await repo.UserSatelliteAdHocLogUserStatistics(config.SatelliteRequestLookbackHours, ct);
        var queueLength = await ows.ServiceBusQueueLength(client, config.SatelliteRequestQueueName, ct);

        return new SatelliteRequestStatisticsModel
        {
            GlobalRequests = stats.Sum(k => k.Value),
            UserRequests = stats.Sum(k => k.Key.Equals(userId, StringComparison.OrdinalIgnoreCase) ? k.Value : 0),
            QueueLength = queueLength,
        };
    }
}
