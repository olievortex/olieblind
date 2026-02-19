using Amazon.S3;
using Azure.Storage.Blobs;
using olieblind.lib.Models;

namespace olieblind.lib.Processes.Interfaces;

public interface ISatelliteRequestProcess
{
    Task Run(SatelliteRequestQueueModel model, BlobContainerClient bronzeClient, IAmazonS3 awsClient, CancellationToken ct);
}
