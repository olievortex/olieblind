using olieblind.lib.DroughtMonitor.Models;

namespace olieblind.lib.DroughtMonitor;

public interface IDroughtMonitorScripting
{
    string CreateDefaultScript(DroughtMonitorProductModel model, CancellationToken ct);
    string CreateDefaultTitle(DroughtMonitorProductModel model);
    string CreateDefaultTranscript(DroughtMonitorProductModel model);
}
