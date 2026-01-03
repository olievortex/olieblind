using Amazon.S3;

namespace olieblind.lib.Processes.Interfaces;

public interface IImportStormEventsSpcProcess
{
    Task Run(AmazonS3Client client, CancellationToken ct);
}
