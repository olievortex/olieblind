using Azure.Storage.Blobs;

namespace olieblind.lib.Processes.Interfaces;

public interface IDeleteOldContentProcess
{
    Task Run(BlobContainerClient bcc, CancellationToken ct);
}
