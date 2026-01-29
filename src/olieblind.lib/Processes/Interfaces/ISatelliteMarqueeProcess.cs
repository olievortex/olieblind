using Amazon.S3;
using Azure.Storage.Blobs;

namespace olieblind.lib.Processes.Interfaces;

public interface ISatelliteMarqueeProcess
{
    Task Run(int year, BlobContainerClient bronzeClient, string goldPath, IAmazonS3 awsClient, CancellationToken ct);
}
