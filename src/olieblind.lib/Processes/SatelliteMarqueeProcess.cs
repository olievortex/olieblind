using Amazon.S3;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using olieblind.data;
using olieblind.lib.Satellite.Interfaces;
using SixLabors.ImageSharp;

namespace olieblind.lib.Processes;

public class SatelliteMarqueeProcess(
    ISatelliteProcess satelliteProcess,
    ISatelliteSource satelliteSource,
    IMyRepository repo)
{
    private readonly Point _finalSize = new(1246, 540);

    public async Task RunAsync(int year, ServiceBusSender sender, Func<int, Task> delayFunc,
        BlobContainerClient bronzeClient, BlobContainerClient goldClient, IAmazonS3 awsClient,
        CancellationToken ct)
    {
        await AnnualProcess(year, delayFunc, sender, bronzeClient, goldClient, awsClient, ct);
        await RequestProcess(goldClient, ct);
    }

    private async Task RequestProcess(BlobContainerClient goldClient, CancellationToken ct)
    {
        var missingPosters = await satelliteSource.GetProductListNoPosterAsync(ct);

        foreach (var missingPoster in missingPosters)
            await satelliteSource.MakePoster(missingPoster, _finalSize, goldClient, ct);
    }

    public async Task AnnualProcess(int year, Func<int, Task> delayFunc, ServiceBusSender sender,
        BlobContainerClient bronzeClient, BlobContainerClient goldClient, IAmazonS3 awsClient, CancellationToken ct)
    {
        var missingPosters = await repo.StormEventsDailySummaryListMissingPostersForYear(year, ct);

        foreach (var missingPoster in missingPosters)
        {
            var satellite = await satelliteProcess.GetMarqueeSatelliteProduct(missingPoster, ct);
            if (satellite is null) continue;

            await satelliteProcess.DownloadSatelliteFile(year, satellite, delayFunc, sender, bronzeClient, awsClient, ct);
            await satelliteProcess.Update1080(satellite, missingPoster, ct);
            await satelliteProcess.CreatePoster(satellite, missingPoster, _finalSize, goldClient, ct);
        }
    }
}