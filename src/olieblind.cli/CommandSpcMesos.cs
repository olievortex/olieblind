using Microsoft.Extensions.Logging;
using olieblind.lib.Processes.Interfaces;
using olieblind.lib.Services;

namespace olieblind.cli;

public class CommandSpcMesos(ILogger<CommandSpcMesos> logger, IOlieConfig config, ISpcMesosProcess spcMesosProcess)
{
    private const string LoggerName = $"olieblind.cli {nameof(CommandSpcMesos)}";

    public async Task<int> Run(OlieArgs args, CancellationToken ct)
    {
        var year = args.IntArg1;

        try
        {
            Console.WriteLine($"{LoggerName} triggered");
            logger.LogInformation("{loggerName} triggered", LoggerName);

            //await RunSatelliteAwsInventoryProcess(ct);
            await spcMesosProcess.Run(year, config.VideoPath, ct);

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{LoggerName} Error: {ex}");
            logger.LogError(ex, "{_loggerName} Error: {error}", LoggerName, ex);

            return 1;
        }
    }

    //private async Task RunSatelliteAwsInventoryProcess(CancellationToken ct)
    //{
    //    using var client = new AmazonS3Client(new AnonymousAWSCredentials(), RegionEndpoint.USEast1);
    //    var process = new SatelliteInventoryProcess(stormyBusiness, satelliteProcess, satelliteSource);
    //    await process.RunAsync(client, ct);
    //}
}