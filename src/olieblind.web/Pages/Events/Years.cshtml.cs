using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using olieblind.data.Models;

namespace olieblind.web.Pages.Events;

public class Years(IHttpClientFactory httpClientFactory) : PageModel
{
    public List<StormEventsAnnualSummaryModel> AnnualList { get; set; } = [];

    public async Task OnGetAsync(CancellationToken ct)
    {
        var httpClient = Program.GetOlieBlue(httpClientFactory);
        var httpResponseMessage = await httpClient.GetAsync($"api/events/annualSummaryList", ct);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            var body = await httpResponseMessage.Content.ReadAsStringAsync(ct);
            AnnualList = JsonConvert.DeserializeObject<List<StormEventsAnnualSummaryModel>>(body) ?? [];
        }
    }
}