using Amazon.S3;
using Azure.Storage.Blobs;
using olieblind.data;
using olieblind.lib.Processes.Interfaces;
using olieblind.lib.Satellite.Interfaces;
using olieblind.lib.Services;

namespace olieblind.lib.Processes;

public class SatelliteMarqueeProcess(
    ISatelliteImageBusiness business,
    IOlieConfig config,
    IMyRepository repo) : ISatelliteMarqueeProcess
{
    public async Task Run(int year, BlobContainerClient bronzeClient, IAmazonS3 amazonS3Client, CancellationToken ct)
    {
        var dailySummaries = await repo.StormEventsDailySummaryListMissingPostersForYear(year, ct);
        var source = business.CreateSatelliteSource(year, amazonS3Client);

        foreach (var dailySummary in dailySummaries)
        {
            var product = await business.GetMarqueeProduct(dailySummary, ct);
            if (product is null) continue;

            await business.DownloadProduct(product, source, bronzeClient, ct);
            await business.Make1080(product, config, ct);
            await business.UpdateDailySummary1080(product, dailySummary, ct);
            await business.MakePoster(product, OlieCommon.SatelliteThumbnailSize, config.VideoPath, ct);
            await business.UpdateDailySummaryPoster(product, dailySummary, ct);
        }
    }
}