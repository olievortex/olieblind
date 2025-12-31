using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using olieblind.lib.Models;

namespace olieblind.web.Pages.ModelForecast;

public class ListModel(IHttpClientFactory httpClientFactory) : PageModel
{
    public ModelForecastIndexModel? Forecast { get; set; }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken ct)
    {
        var httpClient = Program.GetOlieBlue(httpClientFactory);
        var httpForecastResponse = await httpClient.GetAsync($"api/modelforecast/product?id={id}", ct);

        if (httpForecastResponse.IsSuccessStatusCode)
        {
            var body = await httpForecastResponse.Content.ReadAsStringAsync(ct);

            Forecast = JsonConvert.DeserializeObject<ModelForecastIndexModel>(body);
        }

        return Page();
    }
}
