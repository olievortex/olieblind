using olieblind.lib.Processes.Interfaces;
using olieblind.lib.StormPredictionCenter.Interfaces;

namespace olieblind.lib.Processes;

public class SpcMesosProcess(IMesoProductProcess process, IMesoProductSource source) : ISpcMesosProcess
{
    public async Task Run(int year, bool isUpdateOnly, string goldPath, CancellationToken ct)
    {
        var start = isUpdateOnly ? 0 : await source.GetLatestIdForYear(year, ct);

        for (var index = start + 1; index < 5000; index++)
            if (!await DoSomething(year, index, isUpdateOnly, goldPath, ct))
                break;
    }

    public async Task<bool> DoSomething(int year, int index, bool isUpdateOnly, string goldPath, CancellationToken ct)
    {
        return isUpdateOnly
            ? await process.Update(year, index, ct)
            : await process.Download(year, index, goldPath, ct);
    }
}