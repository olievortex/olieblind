using olieblind.lib.StormPredictionCenter.Models;

namespace olieblind.lib.StormPredictionCenter.Outlooks;

public interface IOutlookProductParsing
{
    List<string> ExtractImageNamesFromHtml(string html, int dayNumber);
    string ExtractNarrativeFromHtml(string html);
    OutlookProductModel ParseNarrative(string narrative, int dayNumber);
}
