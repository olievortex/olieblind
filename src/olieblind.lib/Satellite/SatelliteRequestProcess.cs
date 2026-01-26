using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using olieblind.data;
using olieblind.lib.Satellite.Interfaces;
using olieblind.lib.Satellite.Models;
using olieblind.lib.Services;

namespace olieblind.lib.Satellite;

public class SatelliteRequestProcess(ISatelliteRequestBusiness business, IOlieWebService ows, IOlieConfig config, IMyRepository repo) : ISatelliteRequestProcess
{
    public const string NothingToDoMessage = "There is nothing to process for the selected date.";
    public const string QuotaExceededMessage = "Quota exceeded. Please try again later.";
    public const string SuccessMessage = "Satellite request submitted successfully.";

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

    public async Task<SatelliteRequestResultModel> RequestHourlySatellite(string userId, string effectiveDate, string sourceFk, ServiceBusSender sender, CancellationToken ct)
    {
        var productList = await business.GetHourlyProductList(effectiveDate, ct);
        if (productList.Count == 0) return new SatelliteRequestResultModel(false, NothingToDoMessage);

        var isFreeRequest = await business.IsFreeRequest(effectiveDate, sourceFk, ct);

        if (!isFreeRequest && await business.IsQuotaAvailable(userId, ct) is false)
            return new SatelliteRequestResultModel(false, QuotaExceededMessage);

        await business.Enqueue(productList, sender, ct);
        await business.CreateLog(userId, effectiveDate, isFreeRequest, ct);

        return new SatelliteRequestResultModel(true, SuccessMessage);
    }
}
