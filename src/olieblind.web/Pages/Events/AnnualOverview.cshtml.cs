using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using olieblind.lib.StormEvents.Models;

namespace olieblind.web.Pages.Events;

public class AnnualOverview(IHttpClientFactory client) : PageModel
{
    public int Year { get; set; }
    public AnnualOverviewModel Overview { get; set; } = new();
    [BindProperty] public DateTime SelectedDate { get; set; }

    public async Task<ActionResult> OnGetAsync(int year, CancellationToken ct)
    {
        if (year < OlieWebCommon.EventsStart || year > DateTime.UtcNow.Year)
            return RedirectToPage("/Events/Years");

        await InitializePage(year, ct);

        return Page();
    }

    /// <summary>
    /// Load daily overview for user specified date
    /// </summary>
    /// <param name="year">Year for annual summary if there is a problem with redirecting</param>
    /// <param name="ct">Cancellation Token</param>
    /// <returns>Redirect to DailyOverview Page if successful, Annual Overview page if failure</returns>
    public async Task<ActionResult> OnPostAsync(int year, CancellationToken ct)
    {
        if (year < OlieWebCommon.EventsStart || year > DateTime.UtcNow.Year)
            return RedirectToPage("/Events/Years");

        if (!ModelState.IsValid)
        {
            await InitializePage(year, ct);
            return Page();
        }

        var searchDate = $"{SelectedDate.Year}-{SelectedDate.Month:00}-{SelectedDate.Day:00}";
        var identity = await OlieWebCommon.ApiGet< DailyDetailIdentifierModel>(client, $"/api/events/dailyDetailIdentifierByDate/{searchDate}", ct);
        if (identity is null)
        {
            await InitializePage(year, ct);
            ModelState.AddModelError(nameof(SelectedDate), "No severe weather found for this date");
            return Page();
        }

        return RedirectToPage("/Events/DailyOverview", new
        {
            identity.EffectiveDate,
            identity.SourceFk,
            identity.Year
        });
    }

    public async Task InitializePage(int year, CancellationToken ct)
    {
        Overview = await OlieWebCommon.ApiGet<AnnualOverviewModel>(client, $"api/events/annualOverview/{year}", ct) ?? new();
        Year = year;
        SelectedDate = new DateTime(year, 5, 1);

        if (SelectedDate > DateTime.UtcNow) SelectedDate = DateTime.UtcNow.Date;
    }
}