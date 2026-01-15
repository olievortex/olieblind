using Microsoft.AspNetCore.Http.HttpResults;
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
}
