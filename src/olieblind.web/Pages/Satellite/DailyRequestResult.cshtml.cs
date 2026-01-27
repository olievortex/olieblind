using Microsoft.AspNetCore.Mvc.RazorPages;

namespace olieblind.web.Pages.Satellite;

public class DailyRequestResultModel : PageModel
{
    public int Year { get; set; }
    public string EffectiveDate { get; set; } = string.Empty;
    public string SourceFk { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public void OnGet(int year, string effectiveDate, string sourceFk, string message)
    {
        Year = year;
        EffectiveDate = effectiveDate;
        SourceFk = sourceFk;
        Message = message;
    }
}
