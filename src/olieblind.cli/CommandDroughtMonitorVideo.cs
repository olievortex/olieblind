using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using olieblind.lib.Processes.Interfaces;
using olieblind.lib.Services;

namespace olieblind.cli;

public class CommandDroughtMonitorVideo(ILogger<CommandDroughtMonitorVideo> logger, IOlieConfig config, OlieHost host)
{
    private const string _loggerName = $"olieblind.cli {nameof(CommandDroughtMonitorVideo)}";

    public async Task<int> Run(CancellationToken ct)
    {
        try
        {
            Console.WriteLine($"{_loggerName} triggered");
            logger.LogInformation("{loggerName} triggered", _loggerName);

            using var scope = host.ServiceScopeFactory.CreateScope();
            var process = scope.ServiceProvider.GetRequiredService<ICreateDroughtMonitorVideoProcess>();

            await process.Run(config.VideoPath, config.SpeechVoiceName, ct);

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