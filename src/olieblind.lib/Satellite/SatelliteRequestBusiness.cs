using Azure.Messaging.ServiceBus;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Satellite.Interfaces;
using olieblind.lib.Services;

namespace olieblind.lib.Satellite;

public class SatelliteRequestBusiness(IMyRepository repo, IOlieWebService ows, IOlieConfig config) : ISatelliteRequestBusiness
{
    public async Task CreateLog(string userId, string effectiveDate, bool isFree, CancellationToken ct)
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

    public async Task Enqueue(List<SatelliteAwsProductEntity> products, ServiceBusSender sender, CancellationToken ct)
    {
        foreach (var product in products)
        {
            var message = new
            {
                product.Id,
                product.EffectiveDate,
            };

            await ows.ServiceBusSendJson(sender, message, ct);
        }
    }

    public async Task<List<SatelliteAwsProductEntity>> GetHourlyProductList(string effectiveDate, CancellationToken ct)
    {
        var productList = await repo.SatelliteAwsProductGetList(effectiveDate, ct);

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

    public async Task<bool> IsFreeRequest(string effectiveDate, string sourceFk, CancellationToken ct)
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
}
