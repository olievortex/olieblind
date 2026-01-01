using Amazon.S3;
using Azure.Storage.Blobs;

namespace olieblind.lib.Processes;

public interface IImportStormEventsDatabaseProcess
{
    Task RunAsync(int year, BlobContainerClient blobClient, AmazonS3Client amazonClient, CancellationToken ct);
}
