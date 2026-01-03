using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using olieblind.lib.Processes.Interfaces;
using olieblind.lib.Services;

namespace olieblind.cli;

public class CommandEventsDatabase(IImportStormEventsDatabaseProcess process, IOlieConfig config, ILogger<CommandEventsDatabase> logger)
{
    private const string LoggerName = $"olieblind.cli {nameof(CommandEventsDatabase)}";

    public async Task<int> Run(OlieArgs olieArgs, CancellationToken ct)
    {
        var year = olieArgs.IntArg1;
        var update = olieArgs.StrArg1;

        try
        {
            Console.WriteLine($"{LoggerName} triggered for year {year}");
            logger.LogInformation("{loggerName} triggered for year {year}", LoggerName, year);

            var awsClient = new AmazonS3Client(new AnonymousAWSCredentials(), RegionEndpoint.USEast1);
            var blobClient = new BlobContainerClient(new Uri(config.BlobBronzeContainerUri), config.Credential);

            // Run
            await process.Run(year, update, blobClient, awsClient, ct);

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