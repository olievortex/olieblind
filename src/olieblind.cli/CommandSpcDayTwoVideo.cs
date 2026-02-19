using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using olieblind.lib.Processes.Interfaces;
using olieblind.lib.Services;

namespace olieblind.cli;

public class CommandSpcDayTwoVideo(ILogger<CommandSpcDayTwoVideo> logger, IOlieConfig config, OlieHost host)
{
    private const int DayNumber = 2;
    private const string FontName = "Spicy Rice";
    private const string LoggerName = $"olieblind.cli {nameof(CommandSpcDayTwoVideo)}";

    public async Task<int> Run(CancellationToken ct)
    {
        try
        {
            Console.WriteLine($"{LoggerName} triggered");
            logger.LogInformation("{loggerName} triggered", LoggerName);

            using var scope = host.ServiceScopeFactory.CreateScope();
            var process = scope.ServiceProvider.GetRequiredService<ICreateSpcOutlookVideoProcess>();

            await process.Run(
                config.VideoPath,
                FontName,
                config.FontPath,
                config.SpeechVoiceName,
                DayNumber,
                ct);

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