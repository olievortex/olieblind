using Microsoft.Extensions.Logging;
using olieblind.lib.Radar.Interfaces;

namespace olieblind.cli;

public class CommandLoadRadars(IRadarBusiness business, ILogger<CommandLoadRadars> logger)
{
    private const string LoggerName = $"olieblind.cli {nameof(CommandLoadRadars)}";

    public async Task<int> Run(CancellationToken ct)
    {
        try
        {
            Console.WriteLine($"{LoggerName} triggered");
            logger.LogInformation("{loggerName} triggered", LoggerName);

            var value = await File.ReadAllTextAsync("./Resources/nexrad-stations.txt", ct);

            await business.PopulateRadarSitesFromCsvAsync(value, ct);

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