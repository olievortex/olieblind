using olieblind.lib.DroughtMonitor.Models;

namespace olieblind.lib.DroughtMonitor;

public interface IDroughtMonitor
{
    Task<string> GetCurrentDroughtMonitorXmlAsync(CancellationToken ct);
    List<string> GetImageNames();
    string CorrectXmlFormatting(string xml);
    DroughtMonitorProductModel GetProductFromXml(string xml);
}
