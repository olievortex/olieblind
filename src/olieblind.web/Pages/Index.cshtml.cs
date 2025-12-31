using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using olieblind.lib.Models;

namespace olieblind.web.Pages;

public class IndexModel(IHttpClientFactory httpClientFactory) : PageModel
{
    public ProductVideoPageModel? VideoPageModel { get; set; }
    public ModelForecastIndexModel? ForecastIndexModel { get; set; }

    public async Task OnGet(CancellationToken ct)
    {
        var httpClient = Program.GetOlieBlue(httpClientFactory);
        var httpVideoResponse = await httpClient.GetAsync("api/video/page", ct);
        var httpForecastResponse = await httpClient.GetAsync("api/modelforecast/page", ct);

        if (httpVideoResponse.IsSuccessStatusCode)
        {
            var body = await httpVideoResponse.Content.ReadAsStringAsync(ct);

            VideoPageModel = JsonConvert.DeserializeObject<ProductVideoPageModel>(body);
        }

        if (httpForecastResponse.IsSuccessStatusCode)
        {
            var body = await httpForecastResponse.Content.ReadAsStringAsync(ct);

            ForecastIndexModel = JsonConvert.DeserializeObject<ModelForecastIndexModel>(body);
        }
    }
}
