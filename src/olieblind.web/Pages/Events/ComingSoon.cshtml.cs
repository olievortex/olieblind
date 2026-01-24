using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace olieblind.web.Pages.Events;

public class ComingSoon : PageModel
{
    public int Year { get; set; }
    public string EffectiveDate { get; set; } = string.Empty;
    public string SourceFk { get; set; } = string.Empty;

    public void OnGet(int year, string effectiveDate, string sourceFk)
    {
        Year = year;
        EffectiveDate = effectiveDate;
        SourceFk = sourceFk;
    }
}