using Amazon.S3;

namespace olieblind.lib.Processes.Interfaces;

public interface ISatelliteInventoryProcess
{
    Task Run(int year, IAmazonS3 client, CancellationToken ct);
}
