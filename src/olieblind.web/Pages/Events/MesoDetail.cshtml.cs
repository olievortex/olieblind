using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using olieblind.data.Entities;
using olieblind.lib.Services;

namespace olieblind.web.Pages.Events;

public class MesoDetail(IHttpClientFactory client, IOlieConfig config) : PageModel
{
    public int Year { get; set; }
    public string EffectiveDate { get; set; } = string.Empty;
    public string SourceFk { get; set; } = string.Empty;
    public string BlobBase { get; } = config.BaseVideoUrl;
    public SpcMesoProductEntity MesoItem { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int year, int id, string effectiveDate, string sourceFk, CancellationToken ct)
    {
        if (year < OlieWebCommon.EventsStart || year > DateTime.UtcNow.Year)
            return RedirectToPage("/Events/Years");

        var item = await OlieWebCommon.ApiGet<SpcMesoProductEntity>(client, $"/api/events/mesos/{year}/{id}", ct);
        if (item is null)
            return RedirectToPage("/Events/Years");

        Year = year;
        EffectiveDate = effectiveDate;
        SourceFk = sourceFk;
        MesoItem = item;

        return Page();
    }
}