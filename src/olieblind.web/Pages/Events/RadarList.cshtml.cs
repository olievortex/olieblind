using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using olieblind.data.Entities;

namespace olieblind.web.Pages.Events;

public class RadarList(IHttpClientFactory client) : PageModel
{
    public int Year { get; set; }
    public string EffectiveDate { get; set; } = string.Empty;
    public string SourceFk { get; set; } = string.Empty;
    public RadarInventoryEntity? RadarInventory { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int year, string effectiveDate, string sourceFk, string bucketName,
        string radarId, CancellationToken ct)
    {
        if (year < OlieWebCommon.EventsStart || year > DateTime.UtcNow.Year)
            return RedirectToPage("/Events/Years");

        Year = year;
        EffectiveDate = effectiveDate;
        SourceFk = sourceFk;

        RadarInventory = await OlieWebCommon.ApiGet<RadarInventoryEntity>(client, $"/api/events/radar/{radarId}/{effectiveDate}/{bucketName}", ct);

        return Page();
    }

    public string FilenameToTime(string value)
    {
        const string unknown = "Unknown";

        try
        {
            value = value.Replace("_MDM", string.Empty);
            var parts = value.Split('_');
            if (parts.Length != 3) return unknown;

            var time = parts[1];
            return $"{time[..2]}:{time[2..4]}:{time[4..6]}Z";
        }
        catch (Exception)
        {
            return unknown;
        }
    }
}