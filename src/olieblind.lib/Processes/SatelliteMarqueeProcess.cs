using Amazon.S3;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using olieblind.data;
using olieblind.lib.Processes.Interfaces;
using olieblind.lib.Satellite.Interfaces;
using SixLabors.ImageSharp;

namespace olieblind.lib.Processes;

public class SatelliteMarqueeProcess(
    ISatelliteProcess satelliteProcess,
    ISatelliteSource satelliteSource,
    IMyRepository repo) : ISatelliteMarqueeProcess
{
    private readonly Point _finalSize = new(1246, 540);

    public async Task Run(int year, ServiceBusSender sender, Func<int, Task> delayFunc,
        BlobContainerClient bronzeClient, BlobContainerClient goldClient, IAmazonS3 awsClient,
        CancellationToken ct)
    {
        await AnnualProcess(year, delayFunc, sender, bronzeClient, goldClient, awsClient, ct);
        await AdhocProcess(goldClient, ct);
    }

    private async Task AdhocProcess(BlobContainerClient goldClient, CancellationToken ct)
    {
        var missingPosters = await repo.SatelliteAwsProductListNoPoster(ct);

        foreach (var missingPoster in missingPosters)
            await satelliteSource.MakeThumbnail(missingPoster, _finalSize, goldClient, ct);
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
            await satelliteProcess.UpdateDailySummary(satellite, missingPoster, ct);
            await satelliteProcess.CreateThumbnailAndUpdateDailySummary(satellite, missingPoster, _finalSize, goldClient, ct);
        }
    }
}