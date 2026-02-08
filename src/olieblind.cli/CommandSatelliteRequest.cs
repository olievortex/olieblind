using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using olieblind.lib.Processes.Interfaces;
using olieblind.lib.Services;

namespace olieblind.cli;

public class CommandSatelliteRequest(ILogger<CommandSatelliteRequest> logger, ISatelliteRequestProcess process, IOlieConfig config)
{
    private const string LoggerName = $"olieblind.cli {nameof(CommandSatelliteRequest)}";
    public const string MutexName = "Global\\olieblind.cli.CommandSatelliteRequest";

    public async Task<int> Run(CancellationToken ct)
    {
        try
        {
            using var mutex = new Mutex(true, MutexName, out bool createdNew);
            if (!createdNew) return 0;

            Console.WriteLine($"{LoggerName} triggered");
            logger.LogInformation("{loggerName} triggered", LoggerName);

            using var awsClient = new AmazonS3Client(new AnonymousAWSCredentials(), RegionEndpoint.USEast1);
            var blobClient = new BlobContainerClient(new Uri(config.BlobBronzeContainerUri), new DefaultAzureCredential());
            await using var sbClient = new ServiceBusClient(config.ServiceBus, new DefaultAzureCredential());
            await using var receiver = sbClient.CreateReceiver(config.SatelliteRequestQueueName);

            await process.Run(receiver, awsClient, blobClient, ct);

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{LoggerName} Error: {ex}");
            logger.LogError(ex, "{_loggerName} Error: {error}", LoggerName, ex);

            return 1;
        }
    }
}
