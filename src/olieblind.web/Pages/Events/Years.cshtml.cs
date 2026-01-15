using Microsoft.AspNetCore.Mvc.RazorPages;
using olieblind.data.Models;

namespace olieblind.web.Pages.Events;

public class Years(IHttpClientFactory client) : PageModel
{
    public List<StormEventsAnnualSummaryModel> AnnualList { get; set; } = [];

    public async Task OnGetAsync(CancellationToken ct)
    {
        AnnualList = await OlieWebCommon.ApiGet<List<StormEventsAnnualSummaryModel>>(client, "api/events/annualSummaryList", ct)
            ?? [];
    }
}