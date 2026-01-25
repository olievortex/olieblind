using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;

namespace olieblind.web.Pages.Events;

public class MapList : PageModel
{
    public int Year { get; set; }
    public string EffectiveDate { get; set; } = string.Empty;
    public string SourceFk { get; set; } = string.Empty;
    public List<string> SpcLevels = ["sfc", "925", "850", "700", "500", "300", "250"];
    public List<string> SpcLevelNames = ["Surface", "925mb", "850mb", "700mb", "500mb", "300mb", "250mb"];
    public DateTime EffectiveValue;

    public IActionResult OnGet(int year, string effectiveDate, string sourceFk)
    {
        var effectiveValue = FromEffectiveDate(effectiveDate);

        if (year < OlieWebCommon.EventsStart || year > DateTime.UtcNow.Year || !effectiveValue.HasValue)
            return RedirectToPage("/Events/Years");

        Year = year;
        EffectiveDate = effectiveDate;
        SourceFk = sourceFk;
        EffectiveValue = effectiveValue.Value;

        return Page();
    }

    public DateTime? FromEffectiveDate(string value)
    {
        if (!DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.None,
                out var parsed))
            return null;

        return new DateTime(parsed.Year, parsed.Month, parsed.Day, 0, 0, 0, DateTimeKind.Utc);
    }
}