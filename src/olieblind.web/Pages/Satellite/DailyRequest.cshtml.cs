using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using olieblind.lib.Models;
using olieblind.lib.Satellite.Models;
using olieblind.lib.Services;

namespace olieblind.web.Pages.Satellite;

[Authorize]
public class DailyRequest(IHttpClientFactory client, IOlieConfig config) : PageModel
{
    public int Year { get; set; }
    public int GlobalQuota => config.SatelliteRequestGlobalLimit;
    public int UserQuota => config.SatelliteRequestUserLimit;
    public int HoursQuota => config.SatelliteRequestLookbackHours;
    public string EffectiveDate { get; set; } = string.Empty;
    public string SourceFk { get; set; } = string.Empty;
    public SatelliteRequestStatisticsModel? SatelliteRequestStatistics { get; set; }

    public async Task<IActionResult> OnGetAsync(int year, string effectiveDate, string sourceFk, CancellationToken ct)
    {
        if (year < OlieWebCommon.EventsStart || year > DateTime.UtcNow.Year)
            return RedirectToPage("/Events/Years");

        var userId = OlieWebCommon.Email(User);

        if (string.IsNullOrWhiteSpace(userId))
        {
            return new UnauthorizedResult();
        }

        SatelliteRequestStatistics = await OlieWebCommon.ApiGet<SatelliteRequestStatisticsModel>(client, $"/api/satellite/requestStatistics/{userId}", ct);
        Year = year;
        EffectiveDate = effectiveDate;
        SourceFk = sourceFk;

        return Page();
    }

    public async Task<IActionResult> OnPostAllAsync(int year, string effectiveDate, string sourceFk)
    {
        return RedirectToPage("/Events/ComingSoon", new { year, effectiveDate, sourceFk });
    }

    public async Task<IActionResult> OnPostHourlyAsync(int year, string effectiveDate, string sourceFk, CancellationToken ct)
    {
        var model = new SatelliteRequestModel
        {
            EffectiveDate = effectiveDate,
            SourceFk = sourceFk,
            UserId = OlieWebCommon.Email(User) ?? "anonymous"
        };

        var result = await OlieWebCommon.ApiPost<SatelliteRequestResultModel>(client, "/api/satellite/requestHourlyPreview", model, ct);
        if (result is null) throw new ApplicationException("Failed to get a response from the satellite request API.");

        return RedirectToPage("/Satellite/DailyRequestResult", new { year, effectiveDate, sourceFk, result.Message });
    }
}