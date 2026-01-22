using Microsoft.AspNetCore.Mvc.RazorPages;
using olieblind.lib.Models;

namespace olieblind.web.Pages;

public class IndexModel(IHttpClientFactory client) : PageModel
{
    public ProductVideoPageModel? VideoPageModel { get; set; }
    public ModelForecastIndexModel? ForecastIndexModel { get; set; }

    public async Task OnGet(CancellationToken ct)
    {
        VideoPageModel = await OlieWebCommon.ApiGet<ProductVideoPageModel>(client, "/api/video/page", ct);
        ForecastIndexModel = await OlieWebCommon.ApiGet<ModelForecastIndexModel>(client, "/api/modelforecast/page", ct);
    }
}
