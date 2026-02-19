using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using olieblind.lib.Processes.Interfaces;

namespace olieblind.cli;

public class CommandEventsSpc(ILogger<CommandEventsSpc> logger, OlieHost host)
{
    private const string LoggerName = $"olieblind.cli {nameof(CommandEventsSpc)}";

    public async Task<int> Run(OlieArgs args, CancellationToken ct)
    {
        var year = args.IntArg1;

        try
        {
            Console.WriteLine($"{LoggerName} triggered for year {year}");
            logger.LogInformation("{loggerName} triggered for year {year}", LoggerName, year);

            var awsClient = new AmazonS3Client(new AnonymousAWSCredentials(), RegionEndpoint.USEast1);

            using var scope = host.ServiceScopeFactory.CreateScope();
            var process = scope.ServiceProvider.GetRequiredService<IImportStormEventsSpcProcess>();

            await process.Run(year, awsClient, ct);

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
