using Microsoft.Extensions.Logging;
using olieblind.lib.Processes;

namespace olieblind.cli;

public class CommandDayOneMaps(ICreateDayOneMapsProcess process, ILogger<CommandDayOneMaps> logger)
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

            await process.RunAsync(DateOnly.FromDateTime(DateTime.UtcNow), EffectiveHour, ForecastHour, ct);
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