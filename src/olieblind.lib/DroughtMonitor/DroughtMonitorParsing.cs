using System.Xml;

namespace olieblind.lib.DroughtMonitor;

public static class DroughtMonitorParsing
{
    public static XmlElement GetWeekXmlElement(XmlDocument xmlDoc)
    {
        var results = xmlDoc["Results"] ?? throw new Exception("XML file doesn't contain top-level Results tag.");
        var week = results["week"] ?? throw new Exception("XML file does not contain Results::week tag");

        return week;
    }

    public static DateTime GetDate(XmlElement week)
    {
        var element = week["date"] ?? throw new Exception("XML file does not contain Results::week::date");
        var dateText = element.InnerText;
        var year = int.Parse(dateText[..4]);
        var month = int.Parse(dateText[4..6]);
        var day = int.Parse(dateText[6..8]);

        var date = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);

        return date;
    }

    public static string GetInnerXml(string elementName, XmlElement week)
    {
        var element = week[elementName]
                      ?? throw new Exception($"XML file does not contain Results::week::{elementName}");
        var text = element.InnerXml;

        return text;
    }

    public static Dictionary<string, string> GetInnerXmlDict(string tagName, string attributeName, XmlElement week,
        Func<string, string> formatter)
    {
        var result = new Dictionary<string, string>();
        var elements = week.GetElementsByTagName(tagName);

        foreach (XmlElement element in elements)
        {
            var key = element.Attributes[attributeName]?.Value ??
                      throw new Exception($"Unable to find {attributeName} in the {tagName} tag");
            var value = formatter(element.InnerXml);

            result.Add(key, value);
        }

        return result;
    }

    public static string ParagraphsToLineFeeds(string html)
    {
        var text = html.Replace("<p>", string.Empty);
        var parts = text.Split("</p>", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        return string.Join('\n', parts);
    }

    public static XmlDocument ParseXml(string xml)
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xml);
        return xmlDoc;
    }

    public static string GetPageName(string pageDate)
    {
        return $"usdm_summary_{pageDate}.xml";
    }

    public static string GetPageDate(DateTime effectiveDate)
    {
        return $"{effectiveDate:yyyyMMdd}";
    }

    public static int GetLastTuesdayDayOffset(DateTime effectiveDate)
    {
        var dow = effectiveDate.Date.DayOfWeek;

        return dow switch
        {
            DayOfWeek.Thursday => -2,
            DayOfWeek.Friday => -3,
            DayOfWeek.Saturday => -4,
            DayOfWeek.Sunday => -5,
            DayOfWeek.Monday => -6,
            DayOfWeek.Tuesday => -7,
            _ => -8
        };
    }
}