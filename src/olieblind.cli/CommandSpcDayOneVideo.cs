using Microsoft.Extensions.Logging;
using olieblind.lib.Processes.Interfaces;
using olieblind.lib.Services;

namespace olieblind.cli;

public class CommandSpcDayOneVideo(ICreateSpcOutlookVideoProcess process, IOlieConfig config, ILogger<CommandSpcDayOneVideo> logger)
{
    private const int DayNumber = 1;
    private const string FontName = "Spicy Rice";
    private const string LoggerName = $"olieblind.cli {nameof(CommandSpcDayOneVideo)}";

    public async Task<int> Run(CancellationToken ct)
    {
        try
        {
            Console.WriteLine($"{LoggerName} triggered");
            logger.LogInformation("{loggerName} triggered", LoggerName);

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