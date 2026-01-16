using Microsoft.AspNetCore.Http.HttpResults;
using olieblind.data.Entities;
using olieblind.data.Models;
using olieblind.lib.StormEvents.Interfaces;
using olieblind.lib.StormEvents.Models;

namespace olieblind.api.Endpoints;

public static class EventsEndpoint
{
    public static void MapEventsEndpoints(this WebApplication app)
    {
        app.MapGet("/api/events/annualSummaryList", GetAnnualSummaryList);
        app.MapGet("/api/events/annualOverview/{year:int}", GetAnnualOverview);
        app.MapGet("/api/events/dailyDetailIdentifierByDate/{effectiveDate}", GetDailyDetailIdentifierByDate);
        app.MapGet("/api/events/dailyOverview/{effectiveDate}/{sourceFk}", GetDailyOverview);
        app.MapGet("/api/events/mesos/{effectiveDate}", GetSpcMesos);
        app.MapGet("/api/events/mesos/{year:int}/{id:int}", GetSpcMeso);
        app.MapGet("/api/events/radar/{radarId}/{effectiveDate}/{bucketName}", GetRadarInventory);
    }

    public static async Task<Ok<List<StormEventsAnnualSummaryModel>>> GetAnnualSummaryList(IStormEventsSource source, CancellationToken ct)
    {
        var result = await source.GetAnnualSummaryList(ct);

        return TypedResults.Ok(result);
    }

    public static async Task<Ok<AnnualOverviewModel>> GetAnnualOverview(int year, IStormEventsBusiness business, CancellationToken ct)
    {
        var result = await business.GetAnnualOverview(year, ct);

        return TypedResults.Ok(result);
    }

    public static async Task<Ok<DailyDetailIdentifierModel>> GetDailyDetailIdentifierByDate(string effectiveDate, IStormEventsBusiness business, CancellationToken ct)
    {
        var result = await business.GetDailyDetailIdentifierByDate(effectiveDate, ct);
        return TypedResults.Ok(result);
    }

    public static async Task<Ok<DailyOverviewModel>> GetDailyOverview(string effectiveDate, string sourceFk, IStormEventsBusiness business, CancellationToken ct)
    {
        var result = await business.GetDailyOverview(effectiveDate, sourceFk, ct);
        return TypedResults.Ok(result);
    }

    public static async Task<Ok<SpcMesoProductEntity>> GetSpcMeso(int year, int id, IStormEventsSource source, CancellationToken ct)
    {
        var result = await source.GetMeso(year, id, ct);
        return TypedResults.Ok(result);
    }

    public static async Task<Ok<List<SpcMesoProductEntity>>> GetSpcMesos(string effectiveDate, IStormEventsSource source, CancellationToken ct)
    {
        var result = await source.GetMesoList(effectiveDate, ct);
        return TypedResults.Ok(result);
    }

    public static async Task<Ok<RadarInventoryEntity>> GetRadarInventory(string radarId, string effectiveDate, string bucketName, IStormEventsSource source, CancellationToken ct)
    {
        var result = await source.GetRadarInventory(radarId, effectiveDate, bucketName, ct);
        return TypedResults.Ok(result);
    }
}
