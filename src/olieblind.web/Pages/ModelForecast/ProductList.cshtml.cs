using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using olieblind.lib.Models;

namespace olieblind.web.Pages.ModelForecast;

public class ProductListModel(IHttpClientFactory httpClientFactory) : PageModel
{
    public List<ModelForecastIndexModel.HeaderModel> Forecast { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var httpClient = Program.GetOlieBlue(httpClientFactory);
        var httpForecastResponse = await httpClient.GetAsync($"api/modelforecast/list", ct);

        if (httpForecastResponse.IsSuccessStatusCode)
        {
            var body = await httpForecastResponse.Content.ReadAsStringAsync(ct);

            Forecast = JsonConvert.DeserializeObject<List<ModelForecastIndexModel.HeaderModel>>(body) ?? [];
        }

        return Page();
    }
}
