using Azure.Storage.Blobs;
using olieblind.lib.Processes.Interfaces;
using olieblind.lib.StormPredictionCenter.Interfaces;

namespace olieblind.lib.Processes;

public class SpcMesosProcess(IMesoProductProcess process, IMesoProductSource source) : ISpcMesosProcess
{
    public async Task Run(int year, string goldPath, BlobContainerClient bronze, CancellationToken ct)
    {
        var start = await source.GetLatestIdForYear(year, ct);

        for (var index = start + 1; index < 5000; index++)
            if (!await process.Download(year, index, goldPath, bronze, ct))
                break;
    }
}