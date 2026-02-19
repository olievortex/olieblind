using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using olieblind.lib.Processes.Interfaces;

namespace olieblind.cli;

public class CommandSatelliteInventory(ILogger<CommandSatelliteInventory> logger, OlieHost host)
{
    private const string LoggerName = $"olieblind.cli {nameof(CommandSatelliteInventory)}";

    public async Task<int> Run(OlieArgs args, CancellationToken ct)
    {
        var year = args.IntArg1;

        try
        {
            Console.WriteLine($"{LoggerName} triggered");
            logger.LogInformation("{loggerName} triggered", LoggerName);

            using var client = new AmazonS3Client(new AnonymousAWSCredentials(), RegionEndpoint.USEast1);

            using var scope = host.ServiceScopeFactory.CreateScope();
            var process = scope.ServiceProvider.GetRequiredService<ISatelliteInventoryProcess>();

            await process.Run(year, client, ct);

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
