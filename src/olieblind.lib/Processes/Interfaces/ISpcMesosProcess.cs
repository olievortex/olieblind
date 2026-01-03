using Azure.Storage.Blobs;

namespace olieblind.lib.Processes.Interfaces;

public interface ISpcMesosProcess
{
    Task Run(int year, bool isUpdateOnly, BlobContainerClient goldClient, CancellationToken ct);
}
