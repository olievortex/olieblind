using olieblind.lib.StormPredictionCenter.Models;

namespace olieblind.lib.StormPredictionCenter.Interfaces;

public interface IOutlookProductScript
{
    string CreateDefaultScript(OutlookProductModel model, int dayNumber);
    string CreateDefaultTranscript(OutlookProductModel model, int dayNumber);
    string CreateHeadline(OutlookProductModel model, int dayNumber);
}
