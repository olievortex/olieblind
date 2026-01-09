using Amazon.S3;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;

namespace olieblind.lib.Processes.Interfaces;

public interface ISatelliteMarqueeProcess
{
    Task Run(int year, ServiceBusSender sender, Func<int, Task> delayFunc,
        BlobContainerClient bronzeClient, BlobContainerClient goldClient, IAmazonS3 awsClient,
        CancellationToken ct);
}
