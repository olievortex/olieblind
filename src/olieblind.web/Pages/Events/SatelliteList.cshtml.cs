using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using olieblind.lib.Services;
using olieblind.lib.StormEvents.Models;

namespace olieblind.web.Pages.Events;

public class SatelliteList(IHttpClientFactory client, IOlieConfig config) : PageModel
{
    public int Year { get; set; }
    public string EffectiveDate { get; set; } = string.Empty;
    public string SourceFk { get; set; } = string.Empty;
    public string BlobBase { get; } = config.BaseVideoUrl;
    public SatelliteListModel Satellite { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int year, string effectiveDate, string sourceFk, CancellationToken ct)
    {
        if (year < OlieWebCommon.EventsStart || year > DateTime.UtcNow.Year)
            return RedirectToPage("/Events/Years");

        Year = year;
        EffectiveDate = effectiveDate;
        SourceFk = sourceFk;

        Satellite = await OlieWebCommon.ApiGet<SatelliteListModel>(client, $"/api/events/satellite/{effectiveDate}", ct) ?? new();

        return Page();
    }
}