using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Models;
using olieblind.lib.Satellite.Interfaces;
using olieblind.lib.Satellite.Models;
using olieblind.lib.Services;

namespace olieblind.lib.Satellite;

public class SatelliteRequestSource(IMyRepository repo, IOlieWebService ows, IOlieConfig config) : ISatelliteRequestSource
{
    public async Task CreateUserLog(string userId, string effectiveDate, bool isFree, CancellationToken ct)
    {
        var entity = new UserSatelliteAdHocLogEntity
        {
            Id = userId,
            Timestamp = DateTime.UtcNow,
            EffectiveDate = effectiveDate,
            Channel = 2,
            DayPart = DayPartsEnum.Afternoon,
            IsFree = isFree
        };

        await repo.UserSatelliteAdHocLogCreate(entity, ct);
    }

    public async Task<List<SatelliteProductEntity>> GetHourlyProductList(string effectiveDate, CancellationToken ct)
    {
        var productList = await repo.SatelliteProductGetList(effectiveDate, ct);

        var hourly = productList
            .Where(w => w.Path1080 is null || w.PathPoster is null)
            .Select(s => new { s.ScanTime.Hour, s.ScanTime })
            .GroupBy(g => g.Hour)
            .Select(s => s.Min(m => m.ScanTime))
            .ToList();
        var products = productList
            .Where(w => hourly.Contains(w.ScanTime))
            .ToList();

        return products;
    }

    public async Task<SatelliteRequestStatisticsModel> GetRequestStatistics(string userId, ServiceBusAdministrationClient client, CancellationToken ct)
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

    public async Task<bool> IsFreeDay(string effectiveDate, string sourceFk, CancellationToken ct)
    {
        var year = int.Parse(effectiveDate[..4]);
        var dailySummary = await repo.StormEventsDailySummaryGet(effectiveDate, year, sourceFk, ct);

        if (dailySummary is null) return false;

        return dailySummary.F5 > 0 ||
               dailySummary.F4 > 0 ||
               dailySummary.F3 > 0 ||
               dailySummary.F2 > 0;
    }

    public async Task<bool> IsQuotaAvailable(string userId, CancellationToken ct)
    {
        var usage = await repo.UserSatelliteAdHocLogUserStatistics(config.SatelliteRequestLookbackHours, ct);
        var totalRequests = usage.Sum(k => k.Value);
        var userRequests = usage.FirstOrDefault(k => k.Key.Equals(userId, StringComparison.OrdinalIgnoreCase)).Value;

        if ((totalRequests < config.SatelliteRequestGlobalLimit) && (userRequests < config.SatelliteRequestUserLimit))
            return true;

        return false;
    }

    public async Task SendMessage(List<SatelliteProductEntity> products, ServiceBusSender sender, CancellationToken ct)
    {
        foreach (var product in products)
        {
            var message = new SatelliteRequestQueueModel
            {
                Id = product.Id,
                EffectiveDate = product.EffectiveDate,
            };

            await ows.ServiceBusSendJson(sender, message, ct);
        }
    }
}
