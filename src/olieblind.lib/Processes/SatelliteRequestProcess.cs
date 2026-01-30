using Amazon.S3;
using Azure.Messaging.ServiceBus;
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
    IOlieConfig config,
    IOlieWebService ows) : ISatelliteRequestProcess
{
    public async Task Run(ServiceBusReceiver receiver, IAmazonS3 awsClient, BlobContainerClient bronzeClient, CancellationToken ct)
    {
        do
        {
            var (message, model) = await ows.ServiceBusReceiveJson<SatelliteRequestQueueModel>(receiver, ct);
            if (message is null) break;
            if (model is null) throw new ApplicationException("Could not deserialize message from queue");

            await Do(model, bronzeClient, awsClient, ct);

            await ows.ServiceBusCompleteMessage(receiver, message, ct);
        } while (!ct.IsCancellationRequested);
    }

    public async Task Do(SatelliteRequestQueueModel model, BlobContainerClient bronzeClient, IAmazonS3 amazonS3Client, CancellationToken ct)
    {
        var product = await repo.SatelliteProductGet(model.Id, model.EffectiveDate, ct)
            ?? throw new ApplicationException($"Requested product doesn't exist ({model.Id},{model.EffectiveDate})");
        var year = int.Parse(model.EffectiveDate[..4]);
        var source = business.CreateSatelliteSource(year, amazonS3Client);

        await business.DownloadProduct(product, source, bronzeClient, ct);
        await business.Make1080(product, config, ct);
        await business.MakePoster(product, OlieCommon.SatelliteThumbnailSize, config.VideoPath, ct);
    }
}