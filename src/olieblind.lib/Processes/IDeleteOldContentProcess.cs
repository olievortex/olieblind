using Azure.Storage.Blobs;

namespace olieblind.lib.Processes;

public interface IDeleteOldContentProcess
{
    Task Run(BlobContainerClient bcc, CancellationToken ct);
}
