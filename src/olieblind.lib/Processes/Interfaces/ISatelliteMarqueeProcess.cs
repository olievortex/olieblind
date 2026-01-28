using Amazon.S3;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;

namespace olieblind.lib.Processes.Interfaces;

public interface ISatelliteMarqueeProcess
{
    Task Run(int year, ServiceBusSender sender, BlobContainerClient bronzeClient, string goldPath,
        IAmazonS3 awsClient, CancellationToken ct);
}
