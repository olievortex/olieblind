using Microsoft.Extensions.Logging;
using olieblind.lib.Processes;
using olieblind.lib.Services;

namespace olieblind.cli;

public class CommandDroughtMonitorVideo(ICreateDroughtMonitorVideoProcess process, IOlieConfig config, ILogger<CommandDroughtMonitorVideo> logger)
{
    private const string _loggerName = $"olieblind.cli {nameof(CommandDroughtMonitorVideo)}";

    public async Task<int> Run(CancellationToken ct)
    {
        try
        {
            Console.WriteLine($"{_loggerName} triggered");
            logger.LogInformation("{loggerName} triggered", _loggerName);

            await process.RunAsync(config.VideoPath, config.SpeechVoiceName, ct);
            return 0;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{_loggerName} Error: {error}", _loggerName, ex);
            Console.WriteLine($"{_loggerName} Error: {ex}");
            return 1;
        }
    }
}