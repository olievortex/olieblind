using Microsoft.AspNetCore.Http.HttpResults;
using olieblind.lib.Models;
using olieblind.lib.Satellite.Interfaces;
using olieblind.lib.Satellite.Models;
using olieblind.lib.Services;
using olieblind.lib.StormEvents.Interfaces;
using olieblind.lib.StormEvents.Models;

namespace olieblind.api.Endpoints;

public static class SatelliteEndpoint
{
    public static void MapSatelliteEndpoints(this WebApplication app)
    {
        app.MapGet("/api/satellite/requestStatistics/{userId}", GetRequestStatistics);
        app.MapGet("/api/satellite/inventory/{effectiveDate}", GetInventory);
        app.MapPost("/api/satellite/requestHourlyPreview", PostRequestHourlyPreview);
    }

    public static async Task<Ok<SatelliteRequestStatisticsModel>> GetRequestStatistics(string userId, ISatelliteRequestSource source, IOlieConfig config, CancellationToken ct)
    {
        var client = config.ServiceBusAdministrationClient();
        var result = await source.GetRequestStatistics(userId, client, ct);
        return TypedResults.Ok(result);
    }

    public static async Task<Ok<SatelliteListModel>> GetInventory(string effectiveDate, IStormEventsBusiness business, CancellationToken ct)
    {
        var result = await business.GetSatelliteList(effectiveDate, ct);
        return TypedResults.Ok(result);
    }

    public static async Task<Ok<SatelliteRequestResultModel>> PostRequestHourlyPreview(SatelliteRequestModel model, ISatelliteRequestProcess process, IOlieConfig config, CancellationToken ct)
    {
        var sender = config.ServiceBusSender();
        var result = await process.RequestHourlySatellite(model, sender, ct);
        return TypedResults.Ok(result);
    }
}
