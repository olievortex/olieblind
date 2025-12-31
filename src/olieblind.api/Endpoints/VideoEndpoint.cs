using Microsoft.AspNetCore.Http.HttpResults;
using olieblind.lib.Models;
using olieblind.lib.Video;

namespace olieblind.api.Endpoints;

public static class VideoEndpoint
{
    public static void MapVideoEndpoints(this WebApplication app)
    {
        app.MapMethods("/api/video/page", ["HEAD"], HeadVideoPageDefault);
        app.MapGet("/api/video/page", ReadVideoPageDefault);
        app.MapGet("/api/video/list", ReadVideoList);
        app.MapGet("/api/video", ReadVideo);
    }

    public static Ok<ProductVideoPageModel> HeadVideoPageDefault()
    {
        var result = new ProductVideoPageModel();
        return TypedResults.Ok(result);
    }

    public static async Task<Results<Ok<ProductVideoPageModel>, NotFound>> ReadVideoPageDefault(IVideoBusiness business, CancellationToken ct)
    {
        var result = await business.GetIndexPageAsync(ct);

        return TypedResults.Ok(result);
    }

    public static async Task<Results<Ok<List<ProductVideoModel>>, NotFound>> ReadVideoList(string category, IVideoBusiness business, CancellationToken ct)
    {
        var result = await business.GetVideoListAsync(category, ct);

        return TypedResults.Ok(result);
    }

    public static async Task<Results<Ok<ProductVideoModel>, NotFound>> ReadVideo(int id, IVideoBusiness business, CancellationToken ct)
    {
        var result = await business.GetVideoAsync(id, ct);
        if (result is null) return TypedResults.NotFound();

        return TypedResults.Ok(result);
    }
}
