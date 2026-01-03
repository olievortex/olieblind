using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.Logging;
using olieblind.lib.Processes;

namespace olieblind.cli;

public class CommandDailyStormDownload(ILogger<CommandDailyStormDownload> logger, ImportStormEventsSpcProcess stormEventsSpcProcess)
{
    private const string LoggerName = $"olieblind.cli {nameof(CommandDailyStormDownload)}";

    public async Task<int> Run(CancellationToken ct)
    {
        try
        {
            Console.WriteLine($"{LoggerName} triggered");
            logger.LogInformation("{loggerName} triggered", LoggerName);

            await RunImportStormEventsSpcProcess(ct);
            //await RunSatelliteAwsInventoryProcess(ct);
            //await RunSpcMesosProcess(ct);

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{LoggerName} Error: {ex}");
            logger.LogError(ex, "{_loggerName} Error: {error}", LoggerName, ex);

            return 1;
        }
    }

    private async Task RunImportStormEventsSpcProcess(CancellationToken ct)
    {
        var awsClient = new AmazonS3Client(new AnonymousAWSCredentials(), RegionEndpoint.USEast1);
        await stormEventsSpcProcess.Run(awsClient, ct);
    }

    //private async Task RunSatelliteAwsInventoryProcess(CancellationToken ct)
    //{
    //    using var client = new AmazonS3Client(new AnonymousAWSCredentials(), RegionEndpoint.USEast1);
    //    var process = new SatelliteInventoryProcess(stormyBusiness, satelliteProcess, satelliteSource);
    //    await process.RunAsync(client, ct);
    //}

    //private async Task RunSpcMesosProcess(CancellationToken ct)
    //{
    //    var olieConfig = new OlieConfig(configuration);
    //    var credOptions = new DefaultAzureCredentialOptions
    //    {
    //        ExcludeVisualStudioCodeCredential = true,
    //        ExcludeVisualStudioCredential = true
    //    };

    //    var blobClient = new BlobContainerClient(new Uri(olieConfig.OlieBlobGoldContainerUri),
    //        new DefaultAzureCredential(credOptions));
    //    var process = new SpcMesosProcess(mesoProductProcess);
    //    await process.RunAsync(blobClient, true, false, ct);
    //}
}