using Azure.Storage.Blobs;
using olieblind.lib.StormPredictionCenter.Interfaces;

namespace olieblind.lib.Processes;

public class SpcMesosProcess(IMesoProductProcess process, IMesoProductSource source)
{
    public async Task RunAsync(int year, bool isUpdateOnly, BlobContainerClient goldClient, CancellationToken ct)
    {
        var start = isUpdateOnly ? 0 : await source.GetLatestIdForYear(year, ct);

        for (var index = start + 1; index < 5000; index++)
            if (!await DoSomethingAsync(year, index, isUpdateOnly, goldClient, ct))
                break;
    }

    public async Task<bool> DoSomethingAsync(int year, int index, bool isUpdateOnly, BlobContainerClient blobClient, CancellationToken ct)
    {
        return isUpdateOnly
            ? await process.Update(year, index, ct)
            : await process.Download(year, index, blobClient, ct);
    }
}