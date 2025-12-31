using olieblind.lib.DroughtMonitor.Models;
using olieblind.lib.Services;

namespace olieblind.lib.DroughtMonitor;

public class DroughtMonitor(IOlieWebService ows) : IDroughtMonitor
{
    private const string CurrentDroughtMonitorBaseUrl = "https://droughtmonitor.unl.edu/services/data/summary/xml/";

    public async Task<string> GetCurrentDroughtMonitorXmlAsync(CancellationToken ct)
    {
        var currentDate = DateTime.UtcNow;
        var dayOffset = DroughtMonitorParsing.GetLastTuesdayDayOffset(currentDate);
        var effectiveDate = currentDate.Date.AddDays(dayOffset);
        var pageDate = DroughtMonitorParsing.GetPageDate(effectiveDate);
        var pageName = DroughtMonitorParsing.GetPageName(pageDate);

        return await ows.ApiGetString($"{CurrentDroughtMonitorBaseUrl}{pageName}", ct);
    }

    public DroughtMonitorProductModel GetProductFromXml(string xml)
    {
        var correctedXml = CorrectXmlFormatting(xml);
        var xmlDocument = DroughtMonitorParsing.ParseXml(correctedXml);
        var week = DroughtMonitorParsing.GetWeekXmlElement(xmlDocument);
        var regions = DroughtMonitorParsing.GetInnerXmlDict("region", "name", week,
            DroughtMonitorParsing.ParagraphsToLineFeeds);

        var model = new DroughtMonitorProductModel
        {
            EffectiveDate = DroughtMonitorParsing.GetDate(week),
            Intro = DroughtMonitorParsing.ParagraphsToLineFeeds(DroughtMonitorParsing.GetInnerXml("intro", week)),
            Forecast = DroughtMonitorParsing.ParagraphsToLineFeeds(DroughtMonitorParsing.GetInnerXml("forecast", week)),
            Regions = regions
        };

        return model;
    }

    public string CorrectXmlFormatting(string xml)
    {
        xml = xml.Replace("&", "&amp;");
        xml = xml.Replace("<1", "&lt;1"); // (<1 Inch of rain)
        xml = xml.Replace("<2", "&lt;2"); // (<2 Inch of rain)
        xml = xml.Replace("< 1", "&lt; 1"); // (< 10th percentile)
        return xml;
    }

    public List<string> GetImageNames()
    {
        return
        [
            "https://droughtmonitor.unl.edu/data/png/current/current_usdm.png",
            "https://droughtmonitor.unl.edu/data/png/current/current_high_plains_trd.png",
            "https://droughtmonitor.unl.edu/data/png/current/current_midwest_trd.png",
            "https://droughtmonitor.unl.edu/data/png/current/current_south_trd.png",
            "https://droughtmonitor.unl.edu/data/png/current/current_northeast_trd.png",
            "https://droughtmonitor.unl.edu/data/png/current/current_southeast_trd.png",
            "https://droughtmonitor.unl.edu/data/png/current/current_west_trd.png"
        ];
    }
}