using Azure.Storage.Blobs;

namespace olieblind.lib.Processes.Interfaces;

public interface ISpcMesosProcess
{
    Task Run(int year, string goldPath, BlobContainerClient bronze, CancellationToken ct);
}
