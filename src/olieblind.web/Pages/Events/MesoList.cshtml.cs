using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using olieblind.data.Entities;
using olieblind.lib.Services;

namespace olieblind.web.Pages.Events;

public class MesoList(IHttpClientFactory client, IOlieConfig config) : PageModel
{
    public int Year { get; set; }
    public string EffectiveDate { get; set; } = string.Empty;
    public string SourceFk { get; set; } = string.Empty;
    public string BlobBase { get; } = config.BaseVideoUrl;
    public List<SpcMesoProductEntity> MesoItems { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(int year, string effectiveDate, string sourceFk, CancellationToken ct)
    {
        if (year < OlieWebCommon.EventsStart || year > DateTime.UtcNow.Year)
            return RedirectToPage("/Events/Years");

        Year = year;
        EffectiveDate = effectiveDate;
        SourceFk = sourceFk;

        MesoItems = await OlieWebCommon.ApiGet<List<SpcMesoProductEntity>>(client, $"/api/events/mesos/{effectiveDate}", ct) ?? [];

        return Page();
    }
}