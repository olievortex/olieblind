using Amazon.S3;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;

namespace olieblind.lib.Processes.Interfaces;

public interface ISatelliteRequestProcess
{
    Task Run(ServiceBusReceiver receiver, IAmazonS3 awsClient, BlobContainerClient bronzeClient, CancellationToken ct);
}
