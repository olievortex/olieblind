using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using olieblind.lib.Radar.Interfaces;

namespace olieblind.cli;

public class CommandLoadRadars(ILogger<CommandLoadRadars> logger, OlieHost host)
{
    private const string LoggerName = $"olieblind.cli {nameof(CommandLoadRadars)}";

    public async Task<int> Run(CancellationToken ct)
    {
        try
        {
            Console.WriteLine($"{LoggerName} triggered");
            logger.LogInformation("{loggerName} triggered", LoggerName);

            var value = await File.ReadAllTextAsync("./Resources/nexrad-stations.txt", ct);

            using var scope = host.ServiceScopeFactory.CreateScope();
            var process = scope.ServiceProvider.GetRequiredService<IRadarBusiness>();

            await process.PopulateRadarSitesFromCsv(value, ct);

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