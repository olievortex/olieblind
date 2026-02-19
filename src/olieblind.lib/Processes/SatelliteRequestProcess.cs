using Amazon.S3;
using Azure.Storage.Blobs;
using olieblind.data;
using olieblind.lib.Models;
using olieblind.lib.Processes.Interfaces;
using olieblind.lib.Satellite.Interfaces;
using olieblind.lib.Services;

namespace olieblind.lib.Processes;

public class SatelliteRequestProcess(
    ISatelliteImageBusiness business,
    IMyRepository repo,
    IOlieConfig config) : ISatelliteRequestProcess
{
    private const int MaxIterations = 50;

    public async Task Run(SatelliteRequestQueueModel model, BlobContainerClient bronzeClient, IAmazonS3 amazonS3Client, CancellationToken ct)
    {
        var product = await repo.SatelliteProductGet(model.Id, model.EffectiveDate, ct)
            ?? throw new ApplicationException($"Requested product doesn't exist ({model.Id},{model.EffectiveDate})");
        var year = int.Parse(product.EffectiveDate[..4]);
        var source = business.CreateSatelliteSource(year, amazonS3Client);

        await business.DownloadProduct(product, source, bronzeClient, ct);
        await business.Make1080(product, config.PurpleCmdPath, config.VideoPath, ct);
        await business.MakePoster(product, OlieCommon.SatelliteThumbnailSize, config.VideoPath, ct);
    }
}