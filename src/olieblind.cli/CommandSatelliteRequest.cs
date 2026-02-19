using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using olieblind.lib.Models;
using olieblind.lib.Processes.Interfaces;
using olieblind.lib.Services;

namespace olieblind.cli;

public class CommandSatelliteRequest(ILogger<CommandSatelliteRequest> logger, IOlieConfig config, IOlieWebService ows, OlieHost host)
{
    private const string LoggerName = $"olieblind.cli {nameof(CommandSatelliteRequest)}";
    public const string MutexName = "Global\\olieblind.cli.CommandSatelliteRequest";

    public async Task<int> Run(CancellationToken ct)
    {
        try
        {
            var timeout = TimeSpan.FromSeconds(60);

            Console.WriteLine($"{LoggerName} triggered");
            logger.LogInformation("{loggerName} triggered", LoggerName);

            using var awsClient = new AmazonS3Client(new AnonymousAWSCredentials(), RegionEndpoint.USEast1);
            var blobClient = new BlobContainerClient(new Uri(config.BlobBronzeContainerUri), new DefaultAzureCredential());
            await using var sbClient = new ServiceBusClient(config.ServiceBus, new DefaultAzureCredential());
            await using var receiver = sbClient.CreateReceiver(config.SatelliteRequestQueueName);

            do
            {
                // Be a respectful little background worker
                Console.WriteLine("Hi dillon!");
                GC.Collect();

                var message = await ows.ServiceBusReceiveJson<SatelliteRequestQueueModel>(receiver, timeout, ct);
                if (message is null) continue;

                using var scope = host.ServiceScopeFactory.CreateScope();
                var process = scope.ServiceProvider.GetRequiredService<ISatelliteRequestProcess>();

                await process.Run(message.Body, blobClient, awsClient, CancellationToken.None);

                await ows.ServiceBusCompleteMessage(receiver, message, ct);
            } while (!ct.IsCancellationRequested);

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
