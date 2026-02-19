using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using olieblind.lib.Processes.Interfaces;
using olieblind.lib.Services;

namespace olieblind.cli;

public class CommandEventsDatabase(IOlieConfig config, ILogger<CommandEventsDatabase> logger, OlieHost host)
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
            var blobClient = new BlobContainerClient(new Uri(config.BlobBronzeContainerUri), new DefaultAzureCredential());

            using var scope = host.ServiceScopeFactory.CreateScope();
            var process = scope.ServiceProvider.GetRequiredService<IImportStormEventsDatabaseProcess>();

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