using Amazon.S3;
using Azure.Storage.Blobs;

namespace olieblind.lib.Processes;

public interface IImportStormEventsDatabaseProcess
{
    Task Run(int year, string update, BlobContainerClient blobClient, AmazonS3Client amazonClient, CancellationToken ct);
}
