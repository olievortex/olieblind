using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using olieblind.lib.Processes.Interfaces;
using olieblind.lib.Services;

namespace olieblind.cli;

public class CommandDeleteOldContent(IDeleteOldContentProcess process, IOlieConfig config, ILogger<CommandDeleteOldContent> logger)
{
    private const string LoggerName = $"olieblind.cli {nameof(CommandDeleteOldContent)}";

    public async Task<int> Run(CancellationToken ct)
    {
        try
        {
            Console.WriteLine($"{LoggerName} triggered");
            logger.LogInformation("{loggerName} triggered", LoggerName);

            var bcc = new BlobContainerClient(config.MySqlBackupContainer, config.Credential);

            await process.Run(bcc, ct);

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
