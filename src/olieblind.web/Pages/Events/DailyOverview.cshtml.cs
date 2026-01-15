using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using olieblind.data.Entities;
using olieblind.lib.Services;
using olieblind.lib.StormEvents.Models;
using olieblind.web.Interfaces;

namespace olieblind.web.Pages.Events;

public class DailyOverview(IHttpClientFactory client, IOlieConfig config) : PageModel, ISortableEventsPage
{
    public string Outline => "btn-outline-secondary";
    public string Solid => "btn-secondary";
    public string LevelIiBucket => "noaa-nexrad-level2";

    public DailyOverviewModel? DailyModel { get; set; }
    public int Year { get; set; }
    public string EffectiveDate { get; set; } = string.Empty;
    public string SourceFk { get; set; } = string.Empty;
    public string BlobBase { get; } = config.BaseVideoUrl;
    public string TimeClass { get; set; } = string.Empty;
    public string MagnitudeClass { get; set; } = string.Empty;
    public string StateClass { get; set; } = string.Empty;
    public string CountyClass { get; set; } = string.Empty;

    public List<StormEventsDailyDetailEntity> EventList { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(int year, string effectiveDate, string sourceFk, string sort, CancellationToken ct)
    {
        if (year < OlieWebCommon.EventsStart || year > DateTime.UtcNow.Year)
            return RedirectToPage("/Events/Years");

        var dailyModel = await OlieWebCommon.ApiGet<DailyOverviewModel>(client, $"/api/events/dailyOverview/{effectiveDate}/{sourceFk}", ct);

        DailyModel = dailyModel;
        Year = year;
        EffectiveDate = effectiveDate;
        SourceFk = sourceFk;
        EventList = dailyModel?.Tornadoes ?? [];

        OlieWebCommon.ProcessSort(this, sort);

        return Page();
    }
}