using Microsoft.AspNetCore.Http.HttpResults;
using olieblind.data.Models;
using olieblind.lib.StormEvents.Interfaces;

namespace olieblind.api.Endpoints;

public static class EventsEndpoint
{
    public static void MapEventsEndpoints(this WebApplication app)
    {
        app.MapGet("/api/events/annualSummaryList", GetAnnualSummaryList);
    }

    public static async Task<Ok<List<StormEventsAnnualSummaryModel>>> GetAnnualSummaryList(IStormEventsSource source, CancellationToken ct)
    {
        var result = await source.GetAnnualSummaryList(ct);

        return TypedResults.Ok(result);
    }
}
