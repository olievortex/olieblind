using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using olieblind.lib.Processes.Interfaces;

namespace olieblind.cli;

public class CommandDayOneMaps(ILogger<CommandDayOneMaps> logger, OlieHost host)
{
    private const string LoggerName = $"olieblind.cli {nameof(CommandDayOneMaps)}";
    private const int EffectiveHour = 0;
    private const int ForecastHour = 18;

    public async Task<int> Run(CancellationToken ct)
    {
        try
        {
            Console.WriteLine($"{LoggerName} triggered");
            logger.LogInformation("{loggerName} triggered", LoggerName);

            using var scope = host.ServiceScopeFactory.CreateScope();
            var process = scope.ServiceProvider.GetRequiredService<ICreateDayOneMapsProcess>();

            await process.Run(DateOnly.FromDateTime(DateTime.UtcNow), EffectiveHour, ForecastHour, ct);

            return 0;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{_loggerName} Error: {error}", LoggerName, ex);
            Console.WriteLine($"{LoggerName} Error: {ex}");
            return 1;
        }
    }
}