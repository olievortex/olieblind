using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using olieblind.lib.Processes.Interfaces;
using olieblind.lib.Services;

namespace olieblind.cli;

public class CommandSatelliteMarquee(ILogger<CommandSatelliteMarquee> logger, ISatelliteMarqueeProcess process, IOlieConfig config)
{
    private const string LoggerName = $"olieblind.cli {nameof(CommandSatelliteMarquee)}";

    public async Task<int> Run(OlieArgs args, CancellationToken ct)
    {
        var year = args.IntArg1;

        try
        {
            Console.WriteLine($"{LoggerName} triggered for year {year}");
            logger.LogInformation("{loggerName} triggered for year {year}", LoggerName, year);

            using var awsClient = new AmazonS3Client(new AnonymousAWSCredentials(), RegionEndpoint.USEast1);
            var blobClient = new BlobContainerClient(new Uri(config.BlobBronzeContainerUri), new DefaultAzureCredential());

            await process.Run(year, blobClient, awsClient, ct);

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
