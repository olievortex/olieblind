using Microsoft.AspNetCore.Http.HttpResults;
using olieblind.lib.ForecastModels;
using olieblind.lib.Models;

namespace olieblind.api.Endpoints;

public static class ModelForecastEndpoint
{
    public static void MapModelForecastEndpoints(this WebApplication app)
    {
        app.MapGet("/api/modelforecast/page", ReadIndex);
        app.MapGet("/api/modelforecast/product", ReadProduct);
        app.MapGet("/api/modelforecast/list", ReadList);
    }

    public static async Task<Ok<ModelForecastIndexModel>> ReadIndex(IModelForecastBusiness business, CancellationToken ct)
    {
        var result = await business.GetIndexPageAsync(ct);

        return TypedResults.Ok(result);
    }

    public static async Task<Results<Ok<ModelForecastIndexModel>, NotFound>> ReadProduct(int id, IModelForecastBusiness business, CancellationToken ct)
    {
        var result = await business.GetProductAsync(id, ct);
        if (result is null) return TypedResults.NotFound();

        return TypedResults.Ok(result);
    }

    public static async Task<Ok<List<ModelForecastIndexModel.HeaderModel>>> ReadList(IModelForecastBusiness business, CancellationToken ct)
    {
        var result = await business.GetProductListAsync(ct);

        return TypedResults.Ok(result);
    }
}
