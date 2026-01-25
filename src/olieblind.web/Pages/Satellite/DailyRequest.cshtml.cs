using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using olieblind.lib.Services;
using olieblind.lib.StormEvents.Models;

namespace olieblind.web.Pages.Satellite;

[Authorize]
public class DailyRequest(IOlieConfig config) : PageModel
{
    public int Year { get; set; }
    public string EffectiveDate { get; set; } = string.Empty;
    public string SourceFk { get; set; } = string.Empty;
    public string BlobBase { get; } = config.BaseVideoUrl;

    public async Task<IActionResult> OnGetAsync(int year, string effectiveDate, string sourceFk, CancellationToken ct)
    {
        if (year < OlieWebCommon.EventsStart || year > DateTime.UtcNow.Year)
            return RedirectToPage("/Events/Years");

        Year = year;
        EffectiveDate = effectiveDate;
        SourceFk = sourceFk;

        return Page();
    }

    public async Task<IActionResult> OnPostAllAsync(int year, string effectiveDate, string sourceFk, CancellationToken ct)
    {
        //await service.GetUserSession(ct);

        //if (string.IsNullOrWhiteSpace(wrapper.Subscription))
        return RedirectToPage("/Events/ComingSoon", new { year, effectiveDate, sourceFk });

        //Satellite = await business.GetSatelliteListAsync(effectiveDate, ct);
        //Year = year;
        //EffectiveDate = effectiveDate;
        //SourceFk = sourceFk;

        //await eventsProcess.RequestHourlySatelliteAsync(effectiveDate, year, ct);

        //return Page();
    }

    public async Task<IActionResult> OnPostHourlyAsync(int year, string effectiveDate, string sourceFk, CancellationToken ct)
    {
        //await service.GetUserSession(ct);

        //if (string.IsNullOrWhiteSpace(wrapper.Subscription))
        return RedirectToPage("/Events/ComingSoon", new { year, effectiveDate, sourceFk });

        //Satellite = await business.GetSatelliteListAsync(effectiveDate, ct);
        //Year = year;
        //EffectiveDate = effectiveDate;
        //SourceFk = sourceFk;

        //await eventsProcess.RequestHourlySatelliteAsync(effectiveDate, year, ct);

        //return Page();
    }
}